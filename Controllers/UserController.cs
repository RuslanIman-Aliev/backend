using Examin_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Examin_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookingContext _context;
        private readonly IConfiguration _configuration;

        public UserController(BookingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            if (user == null) return BadRequest("Invalid user data.");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email || u.Login == user.Login);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email or Login already exists." });
            }

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the user.", details = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email || u.Login == user.Login);

                if (existingUser == null || existingUser.Password != user.Password)
                {
                    return Conflict(new { message = "Invalid login or password" });
                }

                var accessToken = GenerateJwtToken(existingUser);
                var refreshToken = GenerateRefreshToken();

                existingUser.RefreshToken = refreshToken;
                existingUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(45)
                });

                Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new { message = "Login successful!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

 
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))

            {
                Console.WriteLine("Refresh no");

                return Unauthorized("Refresh token not found");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,  
                Expires = DateTime.UtcNow.AddMinutes(45)
            });

            Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, 
                Expires = DateTime.UtcNow.AddDays(7)
            });


            return Ok("Tokens refreshed successfully!");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await _context.SaveChangesAsync();
                }
            }

            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            return Ok("Logged out successfully");
        }

        [EnableCors("AllowSpecificOrigin")]
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            var token = Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Access token not found");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                {
                    return Unauthorized("Invalid token format");
                }

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Invalid token payload");
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
                if (user == null || string.IsNullOrEmpty(user.RefreshToken))
                {
                    return Unauthorized("Session expired or user not found");
                }

                return Ok(new { message = "User is authenticated", userId });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized("Token has expired");
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid or expired token: {ex.Message}");
            }
        }




        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        [HttpGet("GetUsersObject/{id}")]
        public async Task<IActionResult> GetUsersObject(int id)
        {
            var user = await _context.Users
                .Include(u => u.LivingObjects)
                    .ThenInclude(lo => lo.Images)
                .Include(u => u.LivingObjects)
                    .ThenInclude(lo => lo.Address)   
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var objects = user.LivingObjects
                .Where(lo => lo != null)
                .Select(lo => new
                {
                    lo.Id,
                    lo.Name,
                    lo.Description,
                    lo.Price,
                    lo.Square,
                    lo.ObjectType,
                    Images = lo.Images
                        .Where(img => img != null)
                        .Select(img => new
                        {
                            img.Id,
                            img.ImageUrl
                        })
                        .ToList(),
                    Address = lo.Address != null ? new
                    {
                        lo.Address.Street,
                        lo.Address.City,
                        lo.Address.State,
                        lo.Address.PostalCode,
                        lo.Address.Country
                    } : null
                })
                .ToList();

            return Ok(objects);
        }

        [HttpGet("GetUserRole")]
        public async Task<IActionResult> GetUserRole(int id)
        {
            var user = await _context.Users.Include(r => r.UserRoles).FirstOrDefaultAsync(user => user.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var role = user.UserRoles.Select(uo => uo.RoleName).ToList();
            return Ok(role);
        }
        [HttpGet("getUserBookObject/{id}")]
        public async Task<IActionResult> GetUserBookObject(int id)
        {
            var ownerObjects = await _context.LivingObjects
                .Include(lo => lo.Bookings)
                .Where(lo => lo.OwnerId == id)
                .ToListAsync();

            if (!ownerObjects.Any())
            {
                return NotFound("No objects found for this owner.");
            }

            var bookings = ownerObjects
                .Where(lo => lo.Bookings.Any())
                .Select(lo => new
                {
                    ObjectId = lo.Id,
                    ObjectName = lo.Name,
                    Bookings = lo.Bookings.Select(b => new
                    {
                        b.Id,
                        b.DateIn,
                        b.DateOut,
                        b.TotalDayCount,
                        b.TotalNightCount,
                        b.Guests,
                        b.TotalPayingSum
                    }).ToList()
                })
                .ToList();

            if (!bookings.Any())
            {
                return NotFound("No bookings found for this owner.");
            }

            return Ok(bookings);
        }


        [HttpGet("GetUserBookings/{id}")]
        public async Task<IActionResult> GetUserBookings(int id)
        {
            var user = await _context.Users
                .Include(u => u.BookingUsers)
                .ThenInclude(b => b.Object)  
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var bookings = user.BookingUsers
                .Where(b => b != null)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.OwnerId,
                    b.DateIn,
                    b.DateOut,
                    b.TotalDayCount,
                    b.TotalNightCount,
                    b.TotalPayingSum,
                    b.Guests,
                    b.ObjectId,
                    LivingObject = b.Object != null ? new
                    {
                        b.Object.Name,
                        b.Object.Price,
                        b.Object.Id,
                    } : null
                })
                .ToList();

            return Ok(bookings);
        }



        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            //if (User?.Identity?.IsAuthenticated != true)
            //{
            //    return Unauthorized("User is not authenticated");
            //}

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                user.Id,
                user.Login,
                user.Email,
                user.Surname,
                user.Name,
                user.WalletAddress,
            });
        }

        [HttpGet("check-role")]
        public async Task<IActionResult> CheckUserRole()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isAdmin = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleName == "Admin");

            return Ok(new { IsAdmin = isAdmin });
        }
    }

}

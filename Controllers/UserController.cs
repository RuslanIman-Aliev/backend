using Examin_backend.Models;
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
            var userUnique = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
            if (userUnique != null)
            {
                return Conflict("User already exists");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User successfully registered");
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
                    return Conflict("Invalid login or password");
                }

                // Generate tokens
                var accessToken = GenerateJwtToken(existingUser);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token to database
                existingUser.RefreshToken = refreshToken;
                existingUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
           

           
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
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

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
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
        [HttpGet("GetUsersObject")]
        public async Task<IActionResult> GetUsersObject(int id)
        {
            var user = await _context.Users.Include(u=>u.UserObjs).ThenInclude(uo=>uo.Object).FirstAsync(user=>user.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var objects = user.UserObjs.Select(uo => uo.Object).ToList();

            return Ok(objects);
        }
        // роль пользователя 
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
        [HttpGet("GetUserBookings")]
        public async Task<IActionResult> GetUserBookings(int id)
        {
            var user = await _context.Users.Include(r => r.BookingUsers).FirstOrDefaultAsync(user => user.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var bookings = user.BookingUsers.Select(u=>u).ToList();
            return Ok(bookings);
        }
    }

}

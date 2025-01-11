using Examin_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Examin_backend.Class;
using Azure.Core.GeoJson;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using System.Security.Claims;

namespace Examin_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectsController : ControllerBase
    {
        private readonly BookingContext bookingContext;
        private readonly string _apiKey = "AIzaSyCkCp3ps_PnL6lniq1M8lkugeOs6asbljE";

        public ObjectsController(BookingContext context)
        {
            bookingContext = context;
        }
        public async Task<IActionResult> ShowObject()
        {
            var objects = await bookingContext.LivingObjects.ToListAsync();
            return Ok(objects);
        }

        [HttpGet("ShowAllType")]
        public async Task<IActionResult> ShowAllTypeByName(string name)
        {
            IQueryable<LivingObject> query = bookingContext.LivingObjects;

            switch (name?.ToLower())
            {
                case "flat":
                    query = query.Include(o => o.FlatInfos).Where(o => o.FlatInfos != null);
                    break;
                case "house":
                    query = query.Include(o => o.HouseInfos).Where(o => o.HouseInfos != null);
                    break;
                case "hotel":
                    query = query.Include(o => o.HotelInfos).Where(o => o.HotelInfos != null);
                    break;
                case "hostel":
                    query = query.Include(o => o.HostelInfos).Where(o => o.HostelInfos != null);
                    break;
                default:
                    return BadRequest("Invalid object type.");
            }

            var results = await query.ToListAsync();

            if (results == null || !results.Any())
            {
                return NotFound($"No objects found for type: {name}");
            }

            return Ok(results);
        }
        [HttpGet("ShowAllTypeByRating")]
        public async Task<IActionResult> ShowAllTypeByRating()
        {
            var results = await bookingContext.LivingObjects
                .Include(o => o.Reviews)
                .Include(o => o.Special)
                .Include(o => o.Address)
                .Include(o => o.Images)

                .OrderByDescending(o => o.Reviews.Any() ? o.Reviews.Average(re => re.StarsCount) : 0)
                .Select(o => new
                {
                    o.Id,
                    o.ObjectType,
                    o.Price,
                    o.Square,
                    o.Name,
                    Reviews = o.Reviews.Select(re => new
                    {
                        re.StarsCount,
                    }),
                    Address = o.Address != null ? new
                    {
                        o.Address.Street,
                        o.Address.City,
                        o.Address.Country,
                        o.Address.PostalCode
                    } : null,
                    Special = o.Special != null ? new
                    {
                        o.Special.RoomCount,
                        o.Special.MaxPeopleCapacity,
                        o.Special.ToiletCount
                    } : null,
                    Photos = o.Images.Select(img => img.ImageUrl).ToList()
                })
                .ToListAsync();

            if (results == null || !results.Any())
            {
                return NotFound("No objects found");
            }

            return Ok(results);
        }


        [HttpGet("photo/{id}")]
        public async Task<IActionResult> GetPhotoObjectById(int id)
        {
            var result = await bookingContext.Images.Where(o => o.ObjectId == id).Select(o => new { o.ImageUrl })
                .ToListAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetObjectById(int id)
        {
            var result = await bookingContext.LivingObjects
                .Include(o => o.Reviews)
                .Include(o => o.Address)
                .Include(o => o.Special)
                .Include(o => o.Availabilities)   
                .Include(o => o.Bookings)      
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.ObjectType,
                    o.Price,
                    o.Name,
                    o.Square,
                    o.Description,
                    o.OwnerId,
                    Reviews = o.Reviews.Select(re => new
                    {
                        re.StarsCount,
                    }),
                    Address = o.Address != null ? new
                    {
                        o.Address.Street,
                        o.Address.City,
                        o.Address.PostalCode,
                        o.Address.Country,
                    } : null,
                    Special = o.Special != null ? new
                    {
                        o.Special.RoomCount,
                        o.Special.MaxPeopleCapacity,
                        o.Special.ToiletCount
                    } : null,
                    Availabilities = o.Availabilities.Select(a => new
                    {
                        a.DateIn,
                        a.DateOut,
                    }).ToList(),

                    Bookings = o.Bookings.Select(b => new
                        {
                            b.DateIn,
                            b.DateOut,
                        }).ToList(),
                        
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound($"No object found with ID {id}");
            }

            return Ok(result);
        }
        [HttpGet("getlistobjects")]
        public async Task<IActionResult> GetListObjects(string city, DateOnly? dateIn, DateOnly? dateOut, int? guestCount)
        {
            var query = bookingContext.LivingObjects
                .Include(o => o.Address)
                .Include(o => o.Special)
                .Include(o => o.Reviews)
                .Include(o => o.Images).Where(o => o.Address.City == city);

            if (guestCount.HasValue)
            {
                query = query.Where(o => o.Special.MaxPeopleCapacity >= guestCount);
            }

            if (dateIn.HasValue && dateOut.HasValue)
            {
                query = query.Where(o =>
                    o.Availabilities != null &&
                    o.Availabilities.Any(a =>
                        a.DateIn <= dateIn &&   
                        a.DateOut >= dateOut));  
            }



            var result = await query
    .Select(o => new
    {
        o.Id,
        o.Price,
        o.Name,
        o.Square,
        o.ObjectType,
        Reviews = o.Reviews.Select(re => new
        {
           re.StarsCount,
        }),
        Address = o.Address != null ? new
        {
            o.Address.City,
            o.Address.Country,
            o.Address.PostalCode,
            o.Address.Street,
        } : null,
        Availability = o.Availabilities.Select(a => new
        {
            a.DateIn,
            a.DateOut
        }).ToList(),
        Special = o.Special != null ? new
        {
            o.Special.MaxPeopleCapacity,
            o.Special.ToiletCount,
            o.Special.RoomCount,
        } : null,
        Photos = o.Images.Select(img => img.ImageUrl).ToList()
    }).ToListAsync();

            if (result == null || !result.Any())
            {
                return NotFound("No objects found.");
            }

            return Ok(result);

        }


        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyPlaces(string latitude, string longitude,string place)
        {
            
            string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={latitude},{longitude}&radius=3500&type={place}&key={_apiKey}";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return Content(jsonResponse, "application/json");
            
        }
        [HttpPost("book")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto data)
        {
            try
            {
                var existingBooking = await bookingContext.Bookings
                    .Where(b =>  
                                b.UserId == data.UserId &&
                                b.OwnerId == data.OwnerId &&
                                b.DateIn == DateOnly.Parse(data.DateIn) &&
                                b.DateOut == DateOnly.Parse(data.DateOut) &&
                                b.TotalPayingSum == data.TotalSum &&
                                b.TotalDayCount == data.Days &&
                                b.TotalNightCount == data.Night &&
                                b.Guests == data.Guest)
                    .FirstOrDefaultAsync();

                if (existingBooking != null)
                {
                    return Ok( "This booking already exists with the same details." );
                }
                var booking = new Booking
                {
                    UserId = data.UserId,
                    OwnerId = data.OwnerId,
                    DateIn = DateOnly.Parse(data.DateIn),
                    DateOut = DateOnly.Parse(data.DateOut),
                    TotalPayingSum = data.TotalSum,
                    TotalDayCount = data.Days,
                    TotalNightCount = data.Night,
                    Guests = data.Guest,
                    ObjectId = data.ObjectId
                    
                };

                bookingContext.Bookings.Add(booking);
                await bookingContext.SaveChangesAsync();
                var bookingId = booking.Id;
                var payments = new Payment
                {
                    BookingId = bookingId,
                    FromUserId = data.UserId,
                    ToUserId = data.OwnerId,
                    PaymentMethod = "Crypto",
                    PaymentTransaction = data.Hash

                };
                bookingContext.Payments.Add(payments);
                await bookingContext.SaveChangesAsync();
                return Ok(new { Message = "Booking created successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        public bool ConvertToBool(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            value = value.Trim().ToLower();
            return value == "yes";
        }

        [HttpGet("generate-sas-url")]
        public IActionResult GenerateSasUrl(string blobName)
        {
            try
            {
                var decodedBlobName = System.Net.WebUtility.UrlDecode(blobName);

                var blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=bookingimages;AccountKey=8RrCkdhDQG4J+vWYlCxG/9MHO+Xq8/UKDGd13HP0+qMmyRO1VA5GIVqHkntVtS4GwI8bTolTlhKc+AStFfjnAg==;EndpointSuffix=core.windows.net");
                var containerClient = blobServiceClient.GetBlobContainerClient("images-container");
                var blobClient = containerClient.GetBlobClient(decodedBlobName);

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = "images-container",
                    BlobName = decodedBlobName,
                    Resource = "b",   
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);  

                var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(blobServiceClient.AccountName, "8RrCkdhDQG4J+vWYlCxG/9MHO+Xq8/UKDGd13HP0+qMmyRO1VA5GIVqHkntVtS4GwI8bTolTlhKc+AStFfjnAg==")).ToString();
                var sasUrl = blobClient.Uri + "?" + sasToken;

                return Ok(new { sasUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPost("add")]
        public async Task<IActionResult> AddNewObject([FromBody] NewObject data)
        {
            Console.WriteLine($"Data is {data.Description}");

            Console.WriteLine($"Data is {data}");

            if (data == null)
            {
                return BadRequest("Данные не были переданы.");
            }

            try
            {
                var address = new ObjectAddress
                {
                    City = data.City,
                    PostalCode = data.PostalCode,
                    Country = data.Country,
                    Street = data.Street + " " + data.StreetNumber,
                };
                bookingContext.ObjectAddresses.Add(address);

                var specials = new Special
                {
                    Floor = data.Floor,
                    RoomCount = data.RoomCount,
                    MaxPeopleCapacity = data.MaxCapacity,
                    ToiletCount = data.ToiletCount,
                    ParkingInfo = data.Parking,
                    TotalSquare = data.Square,
                    RoomType = data.RoomType,
                };
                bookingContext.Specials.Add(specials);

                await bookingContext.SaveChangesAsync();

                int specialId = specials.Id;
                int addressId = address.Id;

                var livingObject = new LivingObject
                {
                    Description = data.Description,
                    ObjectType = data.ListingType,
                    Name = data.Name,
                    AddressId = addressId,
                    OwnerId = data.OwnerId,
                    SpecialId = specialId,
                    Price = data.Price,
                    Square = data.Square,
                };
                bookingContext.LivingObjects.Add(livingObject);

                await bookingContext.SaveChangesAsync();

                int objectId = livingObject.Id;

                if (data.ImageUrls != null && data.ImageUrls.Any())
                {
                    foreach (var imageUrl in data.ImageUrls)
                    {
                        var newImage = new Image
                        {
                            ObjectId = objectId,         
                            ObjectType = data.ListingType,
                            ImageUrl = imageUrl         
                        };
                        bookingContext.Images.Add(newImage);
                    }
                    await bookingContext.SaveChangesAsync();
                }


                if (data.Book != null && data.Book.Any())
                {
                    foreach (var availability in data.Book)
                    {
                        try
                        {
                            var dateIn = DateOnly.Parse(availability.DateIn);
                            var dateOut = DateOnly.Parse(availability.DateOut);

                            var newAvailability = new Availability
                            {
                                ObjectId = objectId,
                                DateIn = dateIn,
                                DateOut = dateOut,
                                UserId = data.OwnerId
                            };
                            bookingContext.Availabilities.Add(newAvailability);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Ошибка преобразования даты: {ex.Message}");
                        }
                    }
                    await bookingContext.SaveChangesAsync();
                }
                
                switch (data.ListingType)
                {
                    case "Flat":
                        var flatInfo = new FlatInfo
                        {
                            ObjectId = objectId,
                            FlatNumber = data.FlatNumber,
                            IsBalcon = ConvertToBool(data.Balcon),
                            DoorCode = data.DoorCode,
                            HowToGetKey = data.GetKey,
                        };
                        bookingContext.FlatInfos.Add(flatInfo);
                        break;

                    case "House":
                        var houseInfo = new HouseInfo
                        {
                            ObjectId = objectId,
                            IsPoolInclude = ConvertToBool(data.Pool),
                            IsGarage = ConvertToBool(data.Garage),
                            HowToGetKey = data.GetKey,
                            FloatCount = data.FloorCount,
                        };
                        bookingContext.HouseInfos.Add(houseInfo);
                        break;

                    case "Hotel":
                        var hotelInfo = new HotelInfo
                        {
                            ObjectId = objectId,
                            IsTransferInclude = ConvertToBool(data.IsTransfer),
                            RoomNumber = data.RoomNumber,
                        };
                        bookingContext.HotelInfos.Add(hotelInfo);
                        break;

                    case "Hostel":
                        var hostelInfo = new HostelInfo
                        {
                            ObjectId = objectId,
                            PeopleInRoom = data.PeopleInRoom,
                            RoomNumber = data.RoomNumber,
                            ForWho = data.ForWho,
                        };
                        bookingContext.HostelInfos.Add(hostelInfo);
                        break;

                    default:
                        return BadRequest("Неопределённый тип объекта.");
                }
                var userObj = new UserObj
                {
                    ObjectId = objectId,
                    UserId = data.OwnerId,
                };
                bookingContext.UserObjs.Add(userObj);
                await bookingContext.SaveChangesAsync();

                return Ok(new { Message = "Объект успешно создан!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ошибка.", Details = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteObject(int id)
        {
            var livingObject = await bookingContext.LivingObjects.FindAsync(id);

            if (livingObject == null)
            {
                return NotFound();
            }

            var relatedFlats = bookingContext.FlatInfos.Where(f => f.ObjectId == id);
            bookingContext.FlatInfos.RemoveRange(relatedFlats);

            var relatedAvailabilities = bookingContext.Availabilities.Where(a => a.ObjectId == id);
            bookingContext.Availabilities.RemoveRange(relatedAvailabilities);

            var relatedHotels = bookingContext.HotelInfos.Where(h => h.ObjectId == id);
            bookingContext.HotelInfos.RemoveRange(relatedHotels);

            var relatedHouses = bookingContext.HouseInfos.Where(h => h.ObjectId == id);
            bookingContext.HouseInfos.RemoveRange(relatedHouses);

            var relatedHostels = bookingContext.HostelInfos.Where(h => h.ObjectId == id);
            bookingContext.HostelInfos.RemoveRange(relatedHostels);

            bookingContext.LivingObjects.Remove(livingObject);

            await bookingContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("all-bookings")]
        public async Task<IActionResult> GetAllBookings([FromQuery] int userId)
        {
            var isAdmin = await bookingContext.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleName == "Admin");

            if (!isAdmin)
            {
                return Forbid("You do not have permission to access this resource.");
            }

            var livingObjectsWithBookings = await bookingContext.LivingObjects
                .Include(lo => lo.Bookings)  
                .ThenInclude(b => b.User)   
                .Select(lo => new
                {
                    ObjectId = lo.Id,
                    ObjectName = lo.Name,
                    ObjectPrice = lo.Price,
                    Bookings = lo.Bookings.Select(b => new
                    {
                        BookingId = b.Id,
                        User = b.User != null ? new
                        {
                            b.User.Id,
                            b.User.Name,
                            b.User.Email
                        } : null,
                        b.DateIn,
                        b.DateOut,
                        b.TotalPayingSum,
                        b.Guests
                    }).ToList()
                })
                .ToListAsync();

            if (livingObjectsWithBookings == null || !livingObjectsWithBookings.Any())
            {
                return NotFound("No living objects or bookings found.");
            }

            return Ok(livingObjectsWithBookings);
        }

    }
}

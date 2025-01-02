using Examin_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
                    query = query.Include(o => o.Flat).Where(o => o.Flat != null);
                    break;
                case "house":
                    query = query.Include(o => o.House).Where(o => o.House != null);
                    break;
                case "hotel":
                    query = query.Include(o => o.Hotel).Where(o => o.Hotel != null);
                    break;
                case "hostel":
                    query = query.Include(o => o.Hostel).Where(o => o.Hostel != null);
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
           .Include(b => b.ObjectAddresses)
           .Include(r => r.Special)
           .OrderByDescending(o => o.Reviews.StarsCount)
           .Select(o => new
           {
               o.Id,
               o.ObjectType,
               o.Price,
               o.Square,
               Reviews = o.Reviews != null ? new
               {
                   o.Reviews.StarsCount,
               } : null,
               Address = o.ObjectAddresses != null ? o.ObjectAddresses.Select(address => new
               {
                   address.Street,
                   address.City,
                   address.PostalCode
               }).FirstOrDefault() : null,
               Special = o.Special != null ? new
               {
                   o.Special.RoomCount,
                   o.Special.TotalCapacity,
                   o.Special.ToiletCount
               } : null
           })
           .ToListAsync();
            if (results == null || !results.Any())
            {
                return NotFound($"No objects found ");
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
                .Include(o => o.ObjectAddresses)
                .Include(o => o.Special)
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.ObjectType,
                    o.Price,
                    o.Square,
                    o.Name,
                    o.Description,
                    Reviews = o.Reviews != null ? new
                    {
                        o.Reviews.StarsCount,
                    } : null,
                    Address = o.ObjectAddresses.Select(a => new
                    {
                        a.Street,
                        a.City,
                        a.PostalCode,
                        a.Country
                    }).FirstOrDefault(),
                    Special = o.Special != null ? new
                    {
                        o.Special.RoomCount,
                        o.Special.TotalCapacity,
                        o.Special.ToiletCount
                    } : null
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
                .Include(o => o.ObjectAddresses)
                .Include(o => o.Special)
                .Include(o => o.Reviews)
                .Include(o => o.Images) 
                .Where(o => o.ObjectAddresses.Any(address => address.City == city));

            if (guestCount.HasValue)
            {
                query = query.Where(o => o.Specials.Any(count => count.MaxPeopleCapacity >= guestCount));
            }

            if (dateIn.HasValue && dateOut.HasValue)
            {
                query = query.Where(o => o.Availabilities.Any(available => available.DateIn <= dateIn && available.DateOut >= dateOut));
            }

            var result = await query
                .Select(o => new
                {
                    o.Id,
                    o.Price,
                    o.Name,
                    o.Square,
                    o.ObjectType,
                    Reviews = o.Reviews != null ? new
                    {
                        o.Reviews.StarsCount,
                    } : null,
                    Address = o.ObjectAddresses.Select(a => new
                    {
                        a.City,
                        a.Country,
                        a.PostalCode,
                        a.Street,
                    }).FirstOrDefault(),
                    Availability = o.Availabilities.Select(av => new
                    {
                        av.DateIn,
                        av.DateOut
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

    }
}

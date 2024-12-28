//using Examin_backend.Context;
using Examin_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Examin_backend.Controllers
{
    public class HomeController : Controller
    {
       // private readonly BookingContext _bookingContext;
        public async Task<IActionResult> Index()
        {
            //var objects = await _bookingContext.LivingObjects.ToListAsync();
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minitodo.Models;
using System.Diagnostics;

namespace minitodo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext db;
        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            this.db = db;   
        }

        [Authorize]
        public IActionResult Index()
        {
            db.Users.FirstOrDefault();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
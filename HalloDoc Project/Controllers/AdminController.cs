using Microsoft.AspNetCore.Mvc;

namespace HalloDoc_Project.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Partners()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult ProviderLocation()
        {
            return View();
        }

        public IActionResult Providers()
        {
            return View();
        }

        public IActionResult Records()
        {
            return View();
        }
    }
}

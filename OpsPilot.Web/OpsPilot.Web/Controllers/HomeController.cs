using Microsoft.AspNetCore.Mvc;
using OpsPilot.Web.Models;

namespace OpsPilot.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Home";
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using OpsPilot.Web.Models;

namespace OpsPilot.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalRequests = 150,
                PendingRequests = 30,
                ApprovedRequests = 100,
                RejectedRequests = 20,
                ManagerPendingApprovals = 5,
                AdminFinalApprovals = 2
            };

            return View(model);
        }
    }
}
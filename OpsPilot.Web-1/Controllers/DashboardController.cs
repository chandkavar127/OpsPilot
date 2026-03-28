using Microsoft.AspNetCore.Mvc;
using OpsPilot.Web.Models.ViewModels;

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
                ApprovedRequests = 120,
                RejectedRequests = 5,
                ManagerPendingApprovals = 10,
                AdminFinalApprovals = 5
            };

            return View(model);
        }
    }
}
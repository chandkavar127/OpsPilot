using System.Collections.Generic;

namespace OpsPilot.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int ManagerPendingApprovals { get; set; }
        public int AdminFinalApprovals { get; set; }
        public List<RequestSummary> RequestSummaries { get; set; }

        public DashboardViewModel()
        {
            RequestSummaries = new List<RequestSummary>();
        }
    }

    public class RequestSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
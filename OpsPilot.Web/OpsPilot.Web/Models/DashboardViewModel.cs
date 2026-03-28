public class DashboardViewModel
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int ManagerPendingApprovals { get; set; }
    public int AdminFinalApprovals { get; set; }
    
    public List<Request> RecentRequests { get; set; }
}

public class Request
{
    public int Id { get; set; }
    public string RequestType { get; set; }
    public string Status { get; set; }
}
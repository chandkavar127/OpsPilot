namespace OpsPilot.Domain.Enums;

public enum RequestStatus
{
    PendingManagerApproval = 0,
    PendingAdminApproval = 1,
    Approved = 2,
    Rejected = 3
}
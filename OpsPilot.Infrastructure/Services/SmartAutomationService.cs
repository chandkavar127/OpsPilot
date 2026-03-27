using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Infrastructure.Services;

public class SmartAutomationService : ISmartAutomationService
{
    private readonly IRepository<Request> _requestRepository;

    public SmartAutomationService(IRepository<Request> requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public string PredictRequestStatus(Request request)
    {
        if (request.RequestTypeId == 1)
        {
            return "Likely Approved in 1-2 days";
        }

        if (request.Description.Contains("urgent", StringComparison.OrdinalIgnoreCase))
        {
            return "Fast-Track Review";
        }

        return request.Status switch
        {
            RequestStatus.PendingManagerApproval => "Waiting for Manager Review",
            RequestStatus.PendingAdminApproval => "Waiting for Admin Review",
            _ => "Awaiting Update"
        };
    }

    public string DetectTicketCategory(string subject, string description)
    {
        var text = $"{subject} {description}".ToLowerInvariant();
        if (text.Contains("password") || text.Contains("login")) return "Access";
        if (text.Contains("laptop") || text.Contains("hardware")) return "Hardware";
        if (text.Contains("email") || text.Contains("outlook")) return "Email";
        if (text.Contains("vpn") || text.Contains("network")) return "Network";
        return "General";
    }

    public async Task<bool> IsDuplicateRequestAsync(Request request, CancellationToken cancellationToken = default)
    {
        var recent = await _requestRepository.ListAsync(
            x => x.EmployeeProfileId == request.EmployeeProfileId
                 && x.RequestTypeId == request.RequestTypeId
                 && x.Title == request.Title
                 && x.CreatedAtUtc >= DateTime.UtcNow.AddDays(-7),
            cancellationToken);

        return recent.Any();
    }
}
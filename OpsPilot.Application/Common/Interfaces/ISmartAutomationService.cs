using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Common.Interfaces;

public interface ISmartAutomationService
{
    string PredictRequestStatus(Request request);
    string DetectTicketCategory(string subject, string description);
    Task<bool> IsDuplicateRequestAsync(Request request, CancellationToken cancellationToken = default);
}

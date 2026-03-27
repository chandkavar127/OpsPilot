using OpsPilot.Application.DTOs;

namespace OpsPilot.Application.Common.Interfaces;

public interface IRequestService
{
    Task<int> CreateRequestAsync(CreateRequestDto dto, string userId, CancellationToken cancellationToken = default);
    Task<List<RequestSummaryDto>> GetEmployeeRequestsAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<RequestSummaryDto>> GetPendingApprovalsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ExecuteApprovalAsync(ApprovalActionDto dto, CancellationToken cancellationToken = default);
}

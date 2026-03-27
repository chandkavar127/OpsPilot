using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Common.Interfaces;

public interface IWorkflowService
{
    Task<List<ApprovalStep>> BuildApprovalStepsAsync(Request request, CancellationToken cancellationToken = default);
}

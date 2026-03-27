using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.DTOs;
using OpsPilot.Domain.Entities;
using OpsPilot.Domain.Enums;

namespace OpsPilot.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Request> _requestRepository;
    private readonly IRepository<ApprovalStep> _approvalRepository;
    private readonly IRepository<Ticket> _ticketRepository;

    public DashboardService(
        IRepository<Request> requestRepository,
        IRepository<ApprovalStep> approvalRepository,
        IRepository<Ticket> ticketRepository)
    {
        _requestRepository = requestRepository;
        _approvalRepository = approvalRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId, IList<string> roles, CancellationToken cancellationToken = default)
    {
        var requests = await _requestRepository.ListAsync(null, cancellationToken);
        var approvals = await _approvalRepository.ListAsync(null, cancellationToken);
        var tickets = await _ticketRepository.ListAsync(null, cancellationToken);

        var stats = new DashboardStatsDto
        {
            TotalRequests = requests.Count,
            PendingApprovals = approvals.Count(x => x.ApproverUserId == userId && x.Decision == ApprovalDecision.Pending),
            CompletedTasks = approvals.Count(x => x.ApproverUserId == userId && x.Decision != ApprovalDecision.Pending),
            OpenTickets = tickets.Count(x => x.Status is TicketStatus.Open or TicketStatus.InProgress),
            RequestStatusBreakdown = requests
                .GroupBy(x => x.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }
}
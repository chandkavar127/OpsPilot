using OpsPilot.Application.DTOs;

namespace OpsPilot.Application.Common.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(string userId, IList<string> roles, CancellationToken cancellationToken = default);
}

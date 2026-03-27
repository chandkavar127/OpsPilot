namespace OpsPilot.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyAsync(string userId, string title, string message, CancellationToken cancellationToken = default);
    Task NotifyRoleAsync(string role, string title, string message, CancellationToken cancellationToken = default);
}

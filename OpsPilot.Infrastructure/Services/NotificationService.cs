using Microsoft.AspNetCore.SignalR;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task NotifyAsync(string userId, string title, string message, CancellationToken cancellationToken = default)
    {
        await _notificationRepository.AddAsync(new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            CreatedBy = "system"
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", title, message, cancellationToken);
        Console.WriteLine($"[Email-Sim] To {userId}: {title} - {message}");
    }

    public async Task NotifyRoleAsync(string role, string title, string message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(role).SendAsync("ReceiveNotification", title, message, cancellationToken);
        Console.WriteLine($"[Email-Sim][Role:{role}] {title} - {message}");
    }
}
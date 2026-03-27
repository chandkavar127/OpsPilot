using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.DTOs;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Services;

public class TicketService : ITicketService
{
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<EmployeeProfile> _employeeRepository;
    private readonly ISmartAutomationService _smartAutomationService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(
        IRepository<Ticket> ticketRepository,
        IRepository<EmployeeProfile> employeeRepository,
        ISmartAutomationService smartAutomationService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _employeeRepository = employeeRepository;
        _smartAutomationService = smartAutomationService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateTicketAsync(CreateTicketDto dto, string userId, CancellationToken cancellationToken = default)
    {
        var employee = (await _employeeRepository.ListAsync(x => x.UserId == userId, cancellationToken)).FirstOrDefault();
        if (employee is null)
        {
            throw new InvalidOperationException("Employee profile not found.");
        }

        var category = _smartAutomationService.DetectTicketCategory(dto.Subject, dto.Description);
        var ticket = new Ticket
        {
            EmployeeProfileId = employee.Id,
            Subject = dto.Subject,
            Description = dto.Description,
            Category = category,
            Priority = dto.Priority,
            CreatedBy = userId
        };

        await _ticketRepository.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationService.NotifyRoleAsync("IT Support", "New IT Ticket", $"Ticket #{ticket.Id} - {ticket.Subject}", cancellationToken);
        return ticket.Id;
    }

    public async Task<List<Ticket>> GetTicketsAsync(CancellationToken cancellationToken = default)
    {
        return (await _ticketRepository.ListAsync(null, cancellationToken)).OrderByDescending(x => x.CreatedAtUtc).ToList();
    }
}

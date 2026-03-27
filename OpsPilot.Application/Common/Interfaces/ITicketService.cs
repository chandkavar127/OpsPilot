using OpsPilot.Application.DTOs;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Application.Common.Interfaces;

public interface ITicketService
{
    Task<int> CreateTicketAsync(CreateTicketDto dto, string userId, CancellationToken cancellationToken = default);
    Task<List<Ticket>> GetTicketsAsync(CancellationToken cancellationToken = default);
}

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.Common.Mappings;
using OpsPilot.Application.Services;

namespace OpsPilot.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(OpsPilotMappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(OpsPilotMappingProfile).Assembly);
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<ITicketService, TicketService>();
        return services;
    }
}

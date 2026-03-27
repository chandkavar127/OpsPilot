using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Infrastructure.Identity;
using OpsPilot.Infrastructure.Persistence;
using OpsPilot.Infrastructure.Repositories;
using OpsPilot.Infrastructure.Services;

namespace OpsPilot.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=OpsPilotDb;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<OpsPilotDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<OpsPilotDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISmartAutomationService, SmartAutomationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
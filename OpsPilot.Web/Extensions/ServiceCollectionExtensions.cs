using OpsPilot.Application.DependencyInjection;
using OpsPilot.Infrastructure.DependencyInjection;

namespace OpsPilot.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebComposition(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationLayer();
        services.AddInfrastructureLayer(configuration);
        services.AddControllersWithViews();
        services.AddSignalR();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        return services;
    }
}
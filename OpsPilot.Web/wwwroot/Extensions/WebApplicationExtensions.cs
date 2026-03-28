using Microsoft.AspNetCore.Identity;
using OpsPilot.Infrastructure.Identity;
using OpsPilot.Infrastructure.Persistence;
using OpsPilot.Infrastructure.Services;

namespace OpsPilot.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseWebComposition(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHub<NotificationHub>("/hubs/notifications");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");

        return app;
    }

    public static async Task<WebApplication> SeedIdentityAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedData.InitializeAsync(services, userManager, roleManager);

        return app;
    }
}
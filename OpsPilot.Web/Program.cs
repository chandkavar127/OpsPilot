using OpsPilot.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebComposition(builder.Configuration);

var app = builder.Build();

app.UseWebComposition();
await app.SeedIdentityAsync();

app.Run();

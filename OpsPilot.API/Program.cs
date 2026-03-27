using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.DependencyInjection;
using OpsPilot.Infrastructure.DependencyInjection;
using OpsPilot.Infrastructure.Identity;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var key = builder.Configuration["Jwt:Key"] ?? "OpsPilot_SuperSecretKey_For_Demo_Only_2026";
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "OpsPilot.API" }));

app.MapGet("/api/requests", async (IRequestService requestService, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.FindByEmailAsync("employee@opspilot.local");
    if (user is null) return Results.NotFound();
    var data = await requestService.GetEmployeeRequestsAsync(user.Id);
    return Results.Ok(data);
}).RequireAuthorization();

app.Run();

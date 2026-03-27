using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpsPilot.Application.Common.Interfaces;
using OpsPilot.Application.DTOs;
using OpsPilot.Infrastructure.Identity;

namespace OpsPilot.Web.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ITicketService _ticketService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TicketsController(ITicketService ticketService, UserManager<ApplicationUser> userManager)
    {
        _ticketService = ticketService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _ticketService.GetTicketsAsync());
    }

    public IActionResult Create()
    {
        return View(new CreateTicketDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction("Login", "Account");

        await _ticketService.CreateTicketAsync(dto, user.Id);
        return RedirectToAction(nameof(Index));
    }
}
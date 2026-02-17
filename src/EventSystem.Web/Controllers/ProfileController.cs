using EventSystem.Services.Contracts;
using EventSystem.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSystem.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IEventService eventService;
    private readonly IRegistrationService registrationService;

    public ProfileController(IEventService eventService, IRegistrationService registrationService)
    {
        this.eventService = eventService;
        this.registrationService = registrationService;
    }

    public async Task<IActionResult> MyEvents()
    {
        var uid = User.GetUserId();
        if (uid == null) return Challenge();

        var items = await eventService.GetByOrganizerAsync(uid);
        return View(items);
    }

    public async Task<IActionResult> MyRegistrations()
    {
        var uid = User.GetUserId();
        if (uid == null) return Challenge();

        var items = await registrationService.GetMyAsync(uid);
        return View(items);
    }
}

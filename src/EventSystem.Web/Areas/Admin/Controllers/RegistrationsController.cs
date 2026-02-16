using EventSystem.Data.Constants;
using EventSystem.Services.Contracts;
using EventSystem.Services.Exceptions;
using EventSystem.Web.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSystem.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class RegistrationsController : Controller
{
    private readonly IRegistrationService registrationService;

    public RegistrationsController(IRegistrationService registrationService)
        => this.registrationService = registrationService;

    [HttpGet]
    public async Task<IActionResult> ForEvent(int eventId)
        => View(await registrationService.GetForEventAsync(eventId));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, int eventId)
    {
        try
        {
            await registrationService.ApproveAsync(id);
            TempData[TempDataKeys.Success] = "Registration approved.";
        }
        catch (ServiceException ex)
        {
            TempData[TempDataKeys.Error] = ex.Message;
        }

        return RedirectToAction(nameof(ForEvent), new { eventId });
    }
}

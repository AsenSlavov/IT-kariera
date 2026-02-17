using EventSystem.Data.Constants;
using EventSystem.Services.Contracts;
using EventSystem.Services.Exceptions;
using EventSystem.Web.Infrastructure;
using EventSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSystem.Web.Controllers;

public class EventsController : Controller
{
    private readonly IEventService eventService;
    private readonly ILookupsService lookupsService;
    private readonly IRegistrationService registrationService;

    public EventsController(
        IEventService eventService,
        ILookupsService lookupsService,
        IRegistrationService registrationService)
    {
        this.eventService = eventService;
        this.lookupsService = lookupsService;
        this.registrationService = registrationService;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var details = await eventService.GetDetailsAsync(id);
        if (details == null) return NotFound();

        if (!details.IsPublic && !User.IsInRole(Roles.Admin))
        {
            var uid = User.GetUserId();
            if (uid == null || uid != details.OrganizerUserId)
            {
                return Forbid();
            }
        }

        return View(details);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new EventUpsertViewModel
        {
            StartUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1).Date.AddHours(10), DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1).Date.AddHours(12), DateTimeKind.Utc),
            Venues = await lookupsService.GetVenuesAsync(),
            Categories = await lookupsService.GetCategoriesAsync(),
            Tags = await lookupsService.GetTagsAsync(),
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventUpsertViewModel vm)
    {
        vm.Venues = await lookupsService.GetVenuesAsync();
        vm.Categories = await lookupsService.GetCategoriesAsync();
        vm.Tags = await lookupsService.GetTagsAsync();

        if (!ModelState.IsValid) return View(vm);

        var uid = User.GetUserId();
        if (uid == null) return Challenge();

        try
        {
            var id = await eventService.CreateAsync(vm.ToDto(), uid);
            TempData[TempDataKeys.Success] = "Event created.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var details = await eventService.GetDetailsAsync(id);
        if (details == null) return NotFound();

        var canEdit = User.IsInRole(Roles.Admin) || User.GetUserId() == details.OrganizerUserId;
        if (!canEdit) return Forbid();

        var vm = EventUpsertViewModel.FromDetails(details);
        vm.Venues = await lookupsService.GetVenuesAsync();
        vm.Categories = await lookupsService.GetCategoriesAsync();
        vm.Tags = await lookupsService.GetTagsAsync();

        // Preselect by name -> ids via lookups
        // (kept simple; optional improvement: service returns selected IDs)
        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventUpsertViewModel vm)
    {
        vm.Id = id;
        vm.Venues = await lookupsService.GetVenuesAsync();
        vm.Categories = await lookupsService.GetCategoriesAsync();
        vm.Tags = await lookupsService.GetTagsAsync();

        if (!ModelState.IsValid) return View(vm);

        var details = await eventService.GetDetailsAsync(id);
        if (details == null) return NotFound();

        var canEdit = User.IsInRole(Roles.Admin) || User.GetUserId() == details.OrganizerUserId;

        try
        {
            await eventService.UpdateAsync(id, vm.ToDto(), canEdit);
            TempData[TempDataKeys.Success] = "Event updated.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var details = await eventService.GetDetailsAsync(id);
        if (details == null) return NotFound();

        var canDelete = User.IsInRole(Roles.Admin) || User.GetUserId() == details.OrganizerUserId;

        try
        {
            await eventService.DeleteAsync(id, canDelete);
            TempData[TempDataKeys.Success] = "Event deleted.";
            return RedirectToAction("Index", "Home");
        }
        catch (ServiceException ex)
        {
            TempData[TempDataKeys.Error] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var uid = User.GetUserId();
        if (uid == null) return Challenge();

        try
        {
            await registrationService.RegisterAsync(id, uid);
            TempData[TempDataKeys.Success] = "Registration sent (pending).";
        }
        catch (ServiceException ex)
        {
            TempData[TempDataKeys.Error] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var uid = User.GetUserId();
        if (uid == null) return Challenge();

        try
        {
            await registrationService.CancelAsync(id, uid);
            TempData[TempDataKeys.Success] = "Registration cancelled.";
        }
        catch (ServiceException ex)
        {
            TempData[TempDataKeys.Error] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}

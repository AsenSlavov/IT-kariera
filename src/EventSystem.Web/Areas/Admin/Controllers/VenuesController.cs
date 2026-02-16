using EventSystem.Data.Constants;
using EventSystem.Services.Contracts;
using EventSystem.Services.Exceptions;
using EventSystem.Web.Infrastructure;
using EventSystem.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventSystem.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class VenuesController : Controller
{
    private readonly ICrudService crudService;

    public VenuesController(ICrudService crudService) => this.crudService = crudService;

    public async Task<IActionResult> Index()
        => View(await crudService.ListVenuesAsync());

    [HttpGet]
    public IActionResult Create() => View(new VenueEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VenueEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await crudService.CreateVenueAsync(vm.Name, vm.Address, vm.City, vm.Capacity);
            TempData[TempDataKeys.Success] = "Venue created.";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id) => View(new VenueEditViewModel { Id = id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VenueEditViewModel vm)
    {
        vm.Id = id;
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await crudService.UpdateVenueAsync(id, vm.Name, vm.Address, vm.City, vm.Capacity);
            TempData[TempDataKeys.Success] = "Venue updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await crudService.DeleteVenueAsync(id);
            TempData[TempDataKeys.Success] = "Venue deleted.";
        }
        catch (ServiceException ex)
        {
            TempData[TempDataKeys.Error] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

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
public class TagsController : Controller
{
    private readonly ICrudService crudService;

    public TagsController(ICrudService crudService) => this.crudService = crudService;

    public async Task<IActionResult> Index()
        => View(await crudService.ListTagsAsync());

    [HttpGet]
    public IActionResult Create() => View(new NameEditViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NameEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await crudService.CreateTagAsync(vm.Name);
            TempData[TempDataKeys.Success] = "Tag created.";
            return RedirectToAction(nameof(Index));
        }
        catch (ServiceException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id) => View(new NameEditViewModel { Id = id });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NameEditViewModel vm)
    {
        vm.Id = id;
        if (!ModelState.IsValid) return View(vm);

        try
        {
            await crudService.UpdateTagAsync(id, vm.Name);
            TempData[TempDataKeys.Success] = "Tag updated.";
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
            await crudService.DeleteTagAsync(id);
            TempData[TempDataKeys.Success] = "Tag deleted.";
        }
        catch (ServiceException ex)
        {
            TempData[TempDataKeys.Error] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

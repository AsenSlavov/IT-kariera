using EventSystem.Services.Contracts;
using EventSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly IEventService eventService;
    private readonly ILookupsService lookupsService;

    public HomeController(IEventService eventService, ILookupsService lookupsService)
    {
        this.eventService = eventService;
        this.lookupsService = lookupsService;
    }

    public async Task<IActionResult> Index([FromQuery] EventSearchViewModel vm)
    {
        vm.Categories = await lookupsService.GetCategoriesAsync();
        vm.Events = await eventService.GetPublicAsync(vm.Search, vm.City, vm.CategoryId, vm.Sort);
        return View(vm);
    }

    public IActionResult Privacy() => View();
}

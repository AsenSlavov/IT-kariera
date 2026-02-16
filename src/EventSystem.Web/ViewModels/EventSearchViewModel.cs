using EventSystem.Services.Models;

namespace EventSystem.Web.ViewModels;

public sealed class EventSearchViewModel
{
    public string? Search { get; set; }
    public string? City { get; set; }
    public int? CategoryId { get; set; }
    public string Sort { get; set; } = "new";

    public IReadOnlyList<LookupItemDto> Categories { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<EventListItemDto> Events { get; set; } = Array.Empty<EventListItemDto>();
}

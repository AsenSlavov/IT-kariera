using System.ComponentModel.DataAnnotations;
using EventSystem.Services.Models;

namespace EventSystem.Web.ViewModels;

public sealed class EventUpsertViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Start (UTC)")]
    public DateTime StartUtc { get; set; }

    [Required]
    [Display(Name = "End (UTC)")]
    public DateTime EndUtc { get; set; }

    [Range(1, 100000)]
    public int Capacity { get; set; }

    [Display(Name = "Public event")]
    public bool IsPublic { get; set; } = true;

    [Display(Name = "Venue")]
    public int VenueId { get; set; }

    [Display(Name = "Categories")]
    public List<int> CategoryIds { get; set; } = new();

    [Display(Name = "Tags")]
    public List<int> TagIds { get; set; } = new();

    public IReadOnlyList<LookupItemDto> Venues { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Categories { get; set; } = Array.Empty<LookupItemDto>();
    public IReadOnlyList<LookupItemDto> Tags { get; set; } = Array.Empty<LookupItemDto>();

    public EventUpsertDto ToDto()
        => new()
        {
            Title = Title,
            Description = Description,
            StartUtc = DateTime.SpecifyKind(StartUtc, DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(EndUtc, DateTimeKind.Utc),
            Capacity = Capacity,
            IsPublic = IsPublic,
            VenueId = VenueId,
            CategoryIds = CategoryIds,
            TagIds = TagIds
        };

    public static EventUpsertViewModel FromDetails(EventDetailsDto d)
        => new()
        {
            Id = d.Id,
            Title = d.Title,
            Description = d.Description,
            StartUtc = d.StartUtc,
            EndUtc = d.EndUtc,
            Capacity = d.Capacity,
            IsPublic = d.IsPublic,
            VenueId = d.VenueId
        };
}

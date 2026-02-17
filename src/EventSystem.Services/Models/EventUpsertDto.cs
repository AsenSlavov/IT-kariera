using System.ComponentModel.DataAnnotations;

namespace EventSystem.Services.Models;

public sealed class EventUpsertDto
{
    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartUtc { get; set; }

    [Required]
    public DateTime EndUtc { get; set; }

    [Range(1, 100000)]
    public int Capacity { get; set; }

    public bool IsPublic { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int VenueId { get; set; }

    public List<int> CategoryIds { get; set; } = new();

    public List<int> TagIds { get; set; } = new();
}

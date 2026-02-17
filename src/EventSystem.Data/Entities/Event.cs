using System.ComponentModel.DataAnnotations;

namespace EventSystem.Data.Entities;

public class Event
{
    public int Id { get; set; }

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

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int VenueId { get; set; }

    public Venue Venue { get; set; } = null!;

    [Required]
    public string OrganizerUserId { get; set; } = string.Empty;

    public ApplicationUser Organizer { get; set; } = null!;

    public ICollection<EventCategory> EventCategories { get; set; } = new HashSet<EventCategory>();

    public ICollection<EventTag> EventTags { get; set; } = new HashSet<EventTag>();

    public ICollection<Registration> Registrations { get; set; } = new HashSet<Registration>();
}

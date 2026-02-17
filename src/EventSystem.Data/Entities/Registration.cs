using System.ComponentModel.DataAnnotations;

namespace EventSystem.Data.Entities;

public class Registration
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public Event Event { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
}

public enum RegistrationStatus
{
    Pending = 0,
    Approved = 1,
    Cancelled = 2
}

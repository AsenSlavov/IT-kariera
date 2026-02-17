namespace EventSystem.Services.Models;

public sealed class EventDetailsDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; init; }
    public int Capacity { get; init; }
    public bool IsPublic { get; init; }

    public int VenueId { get; init; }
    public string VenueName { get; init; } = string.Empty;
    public string VenueAddress { get; init; } = string.Empty;
    public string VenueCity { get; init; } = string.Empty;
    public int VenueCapacity { get; init; }

    public string OrganizerUserId { get; init; } = string.Empty;
    public string OrganizerEmail { get; init; } = string.Empty;

    public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    public int RegisteredCount { get; init; }
}

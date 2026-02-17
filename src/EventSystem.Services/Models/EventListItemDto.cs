namespace EventSystem.Services.Models;

public sealed class EventListItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; init; }
    public string City { get; init; } = string.Empty;
    public string VenueName { get; init; } = string.Empty;
    public int Capacity { get; init; }
    public int RegisteredCount { get; init; }
    public bool IsPublic { get; init; }
}

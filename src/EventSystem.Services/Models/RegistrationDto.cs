using EventSystem.Data.Entities;

namespace EventSystem.Services.Models;

public sealed class RegistrationDto
{
    public int Id { get; init; }
    public int EventId { get; init; }
    public string EventTitle { get; init; } = string.Empty;
    public DateTime EventStartUtc { get; init; }
    public DateTime RegisteredAtUtc { get; init; }
    public RegistrationStatus Status { get; init; }
}

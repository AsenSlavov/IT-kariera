using EventSystem.Services.Exceptions;
using EventSystem.Services.Implementations;
using EventSystem.Services.Models;
using Xunit;

namespace EventSystem.Tests;

public class EventServiceTests
{
    [Fact]
    public async Task CreateAsync_Throws_When_EndBeforeStart()
    {
        using var db = TestDbFactory.Create(nameof(CreateAsync_Throws_When_EndBeforeStart));
        var service = new EventService(db);

        var dto = new EventUpsertDto
        {
            Title = "X",
            StartUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
            Capacity = 10,
            VenueId = 1,
            CategoryIds = new List<int> { 1 },
            TagIds = new List<int> { 1 }
        };

        await Assert.ThrowsAsync<ServiceException>(() => service.CreateAsync(dto, "user-1"));
    }

    [Fact]
    public async Task CreateAsync_Throws_When_CapacityExceedsVenue()
    {
        using var db = TestDbFactory.Create(nameof(CreateAsync_Throws_When_CapacityExceedsVenue));
        var service = new EventService(db);

        var dto = new EventUpsertDto
        {
            Title = "X",
            StartUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1).AddHours(1), DateTimeKind.Utc),
            Capacity = 999,
            VenueId = 1,
            CategoryIds = new List<int> { 1 },
            TagIds = new List<int> { 1 }
        };

        await Assert.ThrowsAsync<ServiceException>(() => service.CreateAsync(dto, "user-1"));
    }

    [Fact]
    public async Task CreateAsync_Creates_Event()
    {
        using var db = TestDbFactory.Create(nameof(CreateAsync_Creates_Event));
        var service = new EventService(db);

        var dto = new EventUpsertDto
        {
            Title = "My Event",
            StartUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Utc),
            EndUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1).AddHours(2), DateTimeKind.Utc),
            Capacity = 10,
            VenueId = 1,
            CategoryIds = new List<int> { 1 },
            TagIds = new List<int> { 1 }
        };

        var id = await service.CreateAsync(dto, "user-1");
        var details = await service.GetDetailsAsync(id);

        Assert.NotNull(details);
        Assert.Equal("My Event", details!.Title);
    }
}

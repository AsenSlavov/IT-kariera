using EventSystem.Data;
using EventSystem.Services.Contracts;
using EventSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Services.Implementations;

public sealed class LookupsService : ILookupsService
{
    private readonly ApplicationDbContext db;

    public LookupsService(ApplicationDbContext db) => this.db = db;

    public async Task<IReadOnlyList<LookupItemDto>> GetVenuesAsync()
        => await db.Venues
            .OrderBy(v => v.City).ThenBy(v => v.Name)
            .Select(v => new LookupItemDto { Id = v.Id, Name = $"{v.City} — {v.Name} (cap. {v.Capacity})" })
            .ToListAsync();

    public async Task<IReadOnlyList<LookupItemDto>> GetCategoriesAsync()
        => await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new LookupItemDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

    public async Task<IReadOnlyList<LookupItemDto>> GetTagsAsync()
        => await db.Tags
            .OrderBy(t => t.Name)
            .Select(t => new LookupItemDto { Id = t.Id, Name = t.Name })
            .ToListAsync();
}

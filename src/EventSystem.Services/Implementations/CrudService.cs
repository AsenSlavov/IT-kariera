using EventSystem.Data;
using EventSystem.Data.Entities;
using EventSystem.Services.Contracts;
using EventSystem.Services.Exceptions;
using EventSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Services.Implementations;

public sealed class CrudService : ICrudService
{
    private readonly ApplicationDbContext db;

    public CrudService(ApplicationDbContext db) => this.db = db;

    public async Task<IReadOnlyList<LookupItemDto>> ListVenuesAsync()
        => await db.Venues.AsNoTracking()
            .OrderBy(v => v.City).ThenBy(v => v.Name)
            .Select(v => new LookupItemDto { Id = v.Id, Name = $"{v.City} — {v.Name}" })
            .ToListAsync();

    public async Task<int> CreateVenueAsync(string name, string address, string city, int capacity)
    {
        var entity = new Venue
        {
            Name = (name ?? string.Empty).Trim(),
            Address = (address ?? string.Empty).Trim(),
            City = (city ?? string.Empty).Trim(),
            Capacity = capacity
        };

        db.Venues.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateVenueAsync(int id, string name, string address, string city, int capacity)
    {
        var entity = await db.Venues.FirstOrDefaultAsync(v => v.Id == id)
            ?? throw new ServiceException("Venue not found.");

        entity.Name = (name ?? string.Empty).Trim();
        entity.Address = (address ?? string.Empty).Trim();
        entity.City = (city ?? string.Empty).Trim();
        entity.Capacity = capacity;

        await db.SaveChangesAsync();
    }

    public async Task DeleteVenueAsync(int id)
    {
        var entity = await db.Venues.FirstOrDefaultAsync(v => v.Id == id)
            ?? throw new ServiceException("Venue not found.");

        db.Venues.Remove(entity);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<LookupItemDto>> ListCategoriesAsync()
        => await db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new LookupItemDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

    public async Task<int> CreateCategoryAsync(string name)
    {
        var trimmed = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ServiceException("Name is required.");

        var exists = await db.Categories.AnyAsync(c => c.Name == trimmed);
        if (exists) throw new ServiceException("Category name must be unique.");

        var entity = new Category { Name = trimmed };
        db.Categories.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateCategoryAsync(int id, string name)
    {
        var trimmed = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ServiceException("Name is required.");

        var entity = await db.Categories.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ServiceException("Category not found.");

        var exists = await db.Categories.AnyAsync(c => c.Name == trimmed && c.Id != id);
        if (exists) throw new ServiceException("Category name must be unique.");

        entity.Name = trimmed;
        await db.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var entity = await db.Categories.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ServiceException("Category not found.");

        db.Categories.Remove(entity);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<LookupItemDto>> ListTagsAsync()
        => await db.Tags.AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new LookupItemDto { Id = t.Id, Name = t.Name })
            .ToListAsync();

    public async Task<int> CreateTagAsync(string name)
    {
        var trimmed = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ServiceException("Name is required.");

        var exists = await db.Tags.AnyAsync(t => t.Name == trimmed);
        if (exists) throw new ServiceException("Tag name must be unique.");

        var entity = new Tag { Name = trimmed };
        db.Tags.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateTagAsync(int id, string name)
    {
        var trimmed = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ServiceException("Name is required.");

        var entity = await db.Tags.FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new ServiceException("Tag not found.");

        var exists = await db.Tags.AnyAsync(t => t.Name == trimmed && t.Id != id);
        if (exists) throw new ServiceException("Tag name must be unique.");

        entity.Name = trimmed;
        await db.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(int id)
    {
        var entity = await db.Tags.FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new ServiceException("Tag not found.");

        db.Tags.Remove(entity);
        await db.SaveChangesAsync();
    }
}

using EventSystem.Data;
using EventSystem.Data.Entities;
using EventSystem.Services.Contracts;
using EventSystem.Services.Exceptions;
using EventSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Services.Implementations;

public sealed class EventService : IEventService
{
    private readonly ApplicationDbContext db;

    public EventService(ApplicationDbContext db) => this.db = db;

    public async Task<IReadOnlyList<EventListItemDto>> GetPublicAsync(string? search, string? city, int? categoryId, string sort)
    {
        IQueryable<Event> q = db.Events
            .AsNoTracking()
            .Include(e => e.Venue)
            .Include(e => e.Registrations)
            .Include(e => e.EventCategories);

        q = q.Where(e => e.IsPublic);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(e => e.Title.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var c = city.Trim();
            q = q.Where(e => e.Venue.City.Contains(c));
        }

        if (categoryId.HasValue)
        {
            q = q.Where(e => e.EventCategories.Any(ec => ec.CategoryId == categoryId.Value));
        }

        q = sort switch
        {
            "popular" => q.OrderByDescending(e => e.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled))
                          .ThenBy(e => e.StartUtc),
            "soon" => q.OrderBy(e => e.StartUtc),
            _ => q.OrderByDescending(e => e.StartUtc),
        };

        return await q.Select(e => new EventListItemDto
            {
                Id = e.Id,
                Title = e.Title,
                StartUtc = e.StartUtc,
                EndUtc = e.EndUtc,
                City = e.Venue.City,
                VenueName = e.Venue.Name,
                Capacity = e.Capacity,
                RegisteredCount = e.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled),
                IsPublic = e.IsPublic
            })
            .ToListAsync();
    }

    public async Task<EventDetailsDto?> GetDetailsAsync(int id)
    {
        var e = await db.Events
            .AsNoTracking()
            .Include(x => x.Venue)
            .Include(x => x.Organizer)
            .Include(x => x.EventCategories).ThenInclude(ec => ec.Category)
            .Include(x => x.EventTags).ThenInclude(et => et.Tag)
            .Include(x => x.Registrations)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null) return null;

        return new EventDetailsDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartUtc = e.StartUtc,
            EndUtc = e.EndUtc,
            Capacity = e.Capacity,
            IsPublic = e.IsPublic,
            VenueId = e.VenueId,
            VenueName = e.Venue.Name,
            VenueAddress = e.Venue.Address,
            VenueCity = e.Venue.City,
            VenueCapacity = e.Venue.Capacity,
            OrganizerUserId = e.OrganizerUserId,
            OrganizerEmail = e.Organizer.Email ?? string.Empty,
            Categories = e.EventCategories.Select(ec => ec.Category.Name).OrderBy(n => n).ToList(),
            Tags = e.EventTags.Select(et => et.Tag.Name).OrderBy(n => n).ToList(),
            RegisteredCount = e.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled)
        };
    }

    public async Task<int> CreateAsync(EventUpsertDto dto, string organizerUserId)
    {
        ValidateEvent(dto);

        var venue = await db.Venues.FirstOrDefaultAsync(v => v.Id == dto.VenueId)
            ?? throw new ServiceException("Venue not found.");

        if (dto.Capacity > venue.Capacity)
        {
            throw new ServiceException("Event capacity cannot exceed venue capacity.");
        }

        var entity = new Event
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc,
            Capacity = dto.Capacity,
            IsPublic = dto.IsPublic,
            VenueId = dto.VenueId,
            OrganizerUserId = organizerUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        // attach categories/tags
        foreach (var cid in dto.CategoryIds.Distinct())
        {
            entity.EventCategories.Add(new EventCategory { CategoryId = cid });
        }

        foreach (var tid in dto.TagIds.Distinct())
        {
            entity.EventTags.Add(new EventTag { TagId = tid });
        }

        db.Events.Add(entity);
        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(int id, EventUpsertDto dto, bool canEdit)
    {
        if (!canEdit) throw new ServiceException("You do not have permission to edit this event.");

        ValidateEvent(dto);

        var entity = await db.Events
            .Include(e => e.EventCategories)
            .Include(e => e.EventTags)
            .FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new ServiceException("Event not found.");

        var venue = await db.Venues.FirstOrDefaultAsync(v => v.Id == dto.VenueId)
            ?? throw new ServiceException("Venue not found.");

        if (dto.Capacity > venue.Capacity)
        {
            throw new ServiceException("Event capacity cannot exceed venue capacity.");
        }

        entity.Title = dto.Title.Trim();
        entity.Description = dto.Description?.Trim();
        entity.StartUtc = dto.StartUtc;
        entity.EndUtc = dto.EndUtc;
        entity.Capacity = dto.Capacity;
        entity.IsPublic = dto.IsPublic;
        entity.VenueId = dto.VenueId;

        // replace categories/tags
        entity.EventCategories.Clear();
        foreach (var cid in dto.CategoryIds.Distinct())
        {
            entity.EventCategories.Add(new EventCategory { EventId = id, CategoryId = cid });
        }

        entity.EventTags.Clear();
        foreach (var tid in dto.TagIds.Distinct())
        {
            entity.EventTags.Add(new EventTag { EventId = id, TagId = tid });
        }

        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, bool canDelete)
    {
        if (!canDelete) throw new ServiceException("You do not have permission to delete this event.");

        var entity = await db.Events.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new ServiceException("Event not found.");

        db.Events.Remove(entity);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<EventListItemDto>> GetByOrganizerAsync(string organizerUserId)
    {
        return await db.Events
            .AsNoTracking()
            .Include(e => e.Venue)
            .Include(e => e.Registrations)
            .Where(e => e.OrganizerUserId == organizerUserId)
            .OrderByDescending(e => e.StartUtc)
            .Select(e => new EventListItemDto
            {
                Id = e.Id,
                Title = e.Title,
                StartUtc = e.StartUtc,
                EndUtc = e.EndUtc,
                City = e.Venue.City,
                VenueName = e.Venue.Name,
                Capacity = e.Capacity,
                RegisteredCount = e.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled),
                IsPublic = e.IsPublic
            })
            .ToListAsync();
    }

    private static void ValidateEvent(EventUpsertDto dto)
    {
        if (dto.EndUtc <= dto.StartUtc)
        {
            throw new ServiceException("End date/time must be after start date/time.");
        }

        if (dto.StartUtc.Kind != DateTimeKind.Utc || dto.EndUtc.Kind != DateTimeKind.Utc)
        {
            // enforce UTC to avoid time zone confusion in the DB
            throw new ServiceException("StartUtc and EndUtc must be in UTC (DateTimeKind.Utc).");
        }

        if (dto.CategoryIds.Count == 0)
        {
            throw new ServiceException("Select at least one category.");
        }
    }
}

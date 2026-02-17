using EventSystem.Data;
using EventSystem.Data.Entities;
using EventSystem.Services.Contracts;
using EventSystem.Services.Exceptions;
using EventSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Services.Implementations;

public sealed class RegistrationService : IRegistrationService
{
    private readonly ApplicationDbContext db;

    public RegistrationService(ApplicationDbContext db) => this.db = db;

    public async Task RegisterAsync(int eventId, string userId)
    {
        var ev = await db.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == eventId)
            ?? throw new ServiceException("Event not found.");

        if (!ev.IsPublic)
        {
            throw new ServiceException("You cannot register for a private event.");
        }

        var existing = await db.Registrations.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
        if (existing != null && existing.Status != RegistrationStatus.Cancelled)
        {
            throw new ServiceException("You are already registered for this event.");
        }

        var activeCount = ev.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
        if (activeCount >= ev.Capacity)
        {
            throw new ServiceException("Event is full.");
        }

        if (existing != null && existing.Status == RegistrationStatus.Cancelled)
        {
            existing.Status = RegistrationStatus.Pending;
            existing.RegisteredAtUtc = DateTime.UtcNow;
        }
        else
        {
            db.Registrations.Add(new Registration
            {
                EventId = eventId,
                UserId = userId,
                RegisteredAtUtc = DateTime.UtcNow,
                Status = RegistrationStatus.Pending
            });
        }

        await db.SaveChangesAsync();
    }

    public async Task CancelAsync(int eventId, string userId)
    {
        var reg = await db.Registrations.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId)
            ?? throw new ServiceException("Registration not found.");

        reg.Status = RegistrationStatus.Cancelled;
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<RegistrationDto>> GetMyAsync(string userId)
        => await db.Registrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RegisteredAtUtc)
            .Select(r => new RegistrationDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventTitle = r.Event.Title,
                EventStartUtc = r.Event.StartUtc,
                RegisteredAtUtc = r.RegisteredAtUtc,
                Status = r.Status
            })
            .ToListAsync();

    public async Task<IReadOnlyList<RegistrationDto>> GetForEventAsync(int eventId)
        => await db.Registrations
            .AsNoTracking()
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .OrderByDescending(r => r.RegisteredAtUtc)
            .Select(r => new RegistrationDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventTitle = r.Event.Title,
                EventStartUtc = r.Event.StartUtc,
                RegisteredAtUtc = r.RegisteredAtUtc,
                Status = r.Status
            })
            .ToListAsync();

    public async Task ApproveAsync(int registrationId)
    {
        var reg = await db.Registrations
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == registrationId)
            ?? throw new ServiceException("Registration not found.");

        if (reg.Status == RegistrationStatus.Cancelled)
        {
            throw new ServiceException("Cancelled registrations cannot be approved.");
        }

        // Ensure capacity
        var activeCount = await db.Registrations.CountAsync(r => r.EventId == reg.EventId && r.Status != RegistrationStatus.Cancelled);
        if (activeCount > reg.Event.Capacity)
        {
            throw new ServiceException("Event is full.");
        }

        reg.Status = RegistrationStatus.Approved;
        await db.SaveChangesAsync();
    }
}

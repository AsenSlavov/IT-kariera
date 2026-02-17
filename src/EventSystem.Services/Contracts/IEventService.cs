using EventSystem.Services.Models;

namespace EventSystem.Services.Contracts;

public interface IEventService
{
    /// <summary>Returns public events with optional search/filter/sort.</summary>
    Task<IReadOnlyList<EventListItemDto>> GetPublicAsync(string? search, string? city, int? categoryId, string sort);

    /// <summary>Returns event details by id. Non-public events require organizer/admin check in the web layer.</summary>
    Task<EventDetailsDto?> GetDetailsAsync(int id);

    /// <summary>Creates a new event owned by organizerUserId.</summary>
    Task<int> CreateAsync(EventUpsertDto dto, string organizerUserId);

    /// <summary>Updates an event. Caller permissions are enforced via canEdit flag from web layer.</summary>
    Task UpdateAsync(int id, EventUpsertDto dto, bool canEdit);

    /// <summary>Deletes an event. Caller permissions are enforced via canDelete flag from web layer.</summary>
    Task DeleteAsync(int id, bool canDelete);

    /// <summary>Returns events organized by a user.</summary>
    Task<IReadOnlyList<EventListItemDto>> GetByOrganizerAsync(string organizerUserId);
}

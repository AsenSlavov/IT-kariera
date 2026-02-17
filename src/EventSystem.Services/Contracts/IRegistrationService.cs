using EventSystem.Services.Models;

namespace EventSystem.Services.Contracts;

public interface IRegistrationService
{
    Task RegisterAsync(int eventId, string userId);
    Task CancelAsync(int eventId, string userId);

    Task<IReadOnlyList<RegistrationDto>> GetMyAsync(string userId);

    // Admin
    Task<IReadOnlyList<RegistrationDto>> GetForEventAsync(int eventId);
    Task ApproveAsync(int registrationId);
}

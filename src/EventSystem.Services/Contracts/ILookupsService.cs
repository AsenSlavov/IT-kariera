using EventSystem.Services.Models;

namespace EventSystem.Services.Contracts;

public interface ILookupsService
{
    Task<IReadOnlyList<LookupItemDto>> GetVenuesAsync();
    Task<IReadOnlyList<LookupItemDto>> GetCategoriesAsync();
    Task<IReadOnlyList<LookupItemDto>> GetTagsAsync();
}

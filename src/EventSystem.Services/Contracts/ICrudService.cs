using EventSystem.Services.Models;

namespace EventSystem.Services.Contracts;

public interface ICrudService
{
    // Admin CRUD for Venue/Category/Tag
    Task<IReadOnlyList<LookupItemDto>> ListVenuesAsync();
    Task<int> CreateVenueAsync(string name, string address, string city, int capacity);
    Task UpdateVenueAsync(int id, string name, string address, string city, int capacity);
    Task DeleteVenueAsync(int id);

    Task<IReadOnlyList<LookupItemDto>> ListCategoriesAsync();
    Task<int> CreateCategoryAsync(string name);
    Task UpdateCategoryAsync(int id, string name);
    Task DeleteCategoryAsync(int id);

    Task<IReadOnlyList<LookupItemDto>> ListTagsAsync();
    Task<int> CreateTagAsync(string name);
    Task UpdateTagAsync(int id, string name);
    Task DeleteTagAsync(int id);
}

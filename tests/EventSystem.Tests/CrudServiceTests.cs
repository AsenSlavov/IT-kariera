using EventSystem.Services.Exceptions;
using EventSystem.Services.Implementations;
using Xunit;

namespace EventSystem.Tests;

public class CrudServiceTests
{
    [Fact]
    public async Task CreateCategoryAsync_Throws_On_Duplicate()
    {
        using var db = TestDbFactory.Create(nameof(CreateCategoryAsync_Throws_On_Duplicate));
        var service = new CrudService(db);

        await Assert.ThrowsAsync<ServiceException>(() => service.CreateCategoryAsync("Tech"));
    }
}

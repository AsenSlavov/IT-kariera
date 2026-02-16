using EventSystem.Data;
using EventSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Tests;

internal static class TestDbFactory
{
    public static ApplicationDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var db = new ApplicationDbContext(options);

        // Seed minimal lookups
        db.Venues.Add(new Venue { Id = 1, Name = "Test Venue", Address = "Addr", City = "City", Capacity = 100 });
        db.Categories.Add(new Category { Id = 1, Name = "Tech" });
        db.Tags.Add(new Tag { Id = 1, Name = ".NET" });
        db.SaveChanges();

        return db;
    }
}

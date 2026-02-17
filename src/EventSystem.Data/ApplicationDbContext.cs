using EventSystem.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<EventTag> EventTags => Set<EventTag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        builder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        builder.Entity<EventCategory>()
            .HasKey(ec => new { ec.EventId, ec.CategoryId });

        builder.Entity<EventTag>()
            .HasKey(et => new { et.EventId, et.TagId });

        builder.Entity<Registration>()
            .HasIndex(r => new { r.EventId, r.UserId })
            .IsUnique();

        
        builder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Registration>()
            .HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        
        SeedDomain(builder);
    }

    private static void SeedDomain(ModelBuilder builder)
    {
        builder.Entity<Venue>().HasData(
            new Venue { Id = 1, Name = "Tech Hub", Address = "1 Innovation Blvd", City = "Sofia", Capacity = 250 },
            new Venue { Id = 2, Name = "City Arena", Address = "88 Main Street", City = "Plovdiv", Capacity = 1500 },
            new Venue { Id = 3, Name = "Open Air Park", Address = "Park Entrance", City = "Varna", Capacity = 800 }
        );

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Tech" },
            new Category { Id = 2, Name = "Music" },
            new Category { Id = 3, Name = "Sports" },
            new Category { Id = 4, Name = "Education" }
        );

        builder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = ".NET" },
            new Tag { Id = 2, Name = "AI" },
            new Tag { Id = 3, Name = "Concert" },
            new Tag { Id = 4, Name = "Workshop" }
        );
    }
}

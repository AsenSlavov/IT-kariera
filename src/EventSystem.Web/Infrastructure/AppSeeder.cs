using EventSystem.Data;
using EventSystem.Data.Constants;
using EventSystem.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventSystem.Web.Infrastructure;

public sealed class AppSeeder
{
    private const string AdminEmail = "admin@events.local";
    private const string AdminPassword = "Admin123!";

    private readonly ApplicationDbContext db;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public AppSeeder(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        this.db = db;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
                try
        {
            await db.Database.MigrateAsync();
        }
        catch
        {
            // If no migrations exist yet, fall back to EnsureCreated for first run.
            await db.Database.EnsureCreatedAsync();
        }


        await EnsureRolesAsync();
        var adminUser = await EnsureAdminAsync();

        await SeedDemoEventsAsync(adminUser.Id);
    }

    private async Task EnsureRolesAsync()
    {
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(Roles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
        }
    }

    private async Task<ApplicationUser> EnsureAdminAsync()
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == AdminEmail);
        if (user != null)
        {
            if (!await userManager.IsInRoleAsync(user, Roles.Admin))
            {
                await userManager.AddToRoleAsync(user, Roles.Admin);
            }

            return user;
        }

        user = new ApplicationUser
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, AdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to create admin user: " + string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, Roles.Admin);
        return user;
    }

    private async Task SeedDemoEventsAsync(string organizerUserId)
    {
        if (await db.Events.AnyAsync()) return;

        var now = DateTime.UtcNow;

        var e1 = new Event
        {
            Title = ".NET & Azure Workshop",
            Description = "Hands-on workshop: ASP.NET Core, EF Core, Deployment.",
            StartUtc = now.AddDays(7),
            EndUtc = now.AddDays(7).AddHours(3),
            Capacity = 120,
            IsPublic = true,
            VenueId = 1,
            OrganizerUserId = organizerUserId,
            CreatedAtUtc = now
        };

        var e2 = new Event
        {
            Title = "AI Meetup",
            Description = "Talks & networking about practical AI.",
            StartUtc = now.AddDays(14),
            EndUtc = now.AddDays(14).AddHours(2),
            Capacity = 200,
            IsPublic = true,
            VenueId = 1,
            OrganizerUserId = organizerUserId,
            CreatedAtUtc = now
        };

        var e3 = new Event
        {
            Title = "Open Air Concert",
            Description = "Live music in the park.",
            StartUtc = now.AddDays(21),
            EndUtc = now.AddDays(21).AddHours(4),
            Capacity = 600,
            IsPublic = true,
            VenueId = 3,
            OrganizerUserId = organizerUserId,
            CreatedAtUtc = now
        };

        db.Events.AddRange(e1, e2, e3);
        await db.SaveChangesAsync();

        // Categories: Tech=1, Music=2, Education=4. Tags: .NET=1, AI=2, Concert=3, Workshop=4
        db.EventCategories.AddRange(
            new EventCategory { EventId = e1.Id, CategoryId = 1 },
            new EventCategory { EventId = e1.Id, CategoryId = 4 },
            new EventCategory { EventId = e2.Id, CategoryId = 1 },
            new EventCategory { EventId = e3.Id, CategoryId = 2 }
        );

        db.EventTags.AddRange(
            new EventTag { EventId = e1.Id, TagId = 1 },
            new EventTag { EventId = e1.Id, TagId = 4 },
            new EventTag { EventId = e2.Id, TagId = 2 },
            new EventTag { EventId = e3.Id, TagId = 3 }
        );

        await db.SaveChangesAsync();
    }
}

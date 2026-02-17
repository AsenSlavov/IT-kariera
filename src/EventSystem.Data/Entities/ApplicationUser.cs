using Microsoft.AspNetCore.Identity;

namespace EventSystem.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<Event> OrganizedEvents { get; set; } = new HashSet<Event>();

    public ICollection<Registration> Registrations { get; set; } = new HashSet<Registration>();
}

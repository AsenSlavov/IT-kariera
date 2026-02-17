using System.ComponentModel.DataAnnotations;

namespace EventSystem.Data.Entities;

public class Venue
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Range(1, 100000)]
    public int Capacity { get; set; }

    public ICollection<Event> Events { get; set; } = new HashSet<Event>();
}

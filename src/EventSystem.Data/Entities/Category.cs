using System.ComponentModel.DataAnnotations;

namespace EventSystem.Data.Entities;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;

    public ICollection<EventCategory> EventCategories { get; set; } = new HashSet<EventCategory>();
}

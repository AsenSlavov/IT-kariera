using System.ComponentModel.DataAnnotations;

namespace EventSystem.Data.Entities;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;

    public ICollection<EventTag> EventTags { get; set; } = new HashSet<EventTag>();
}

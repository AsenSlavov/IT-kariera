using System.ComponentModel.DataAnnotations;

namespace EventSystem.Web.ViewModels.Admin;

public sealed class NameEditViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;
}

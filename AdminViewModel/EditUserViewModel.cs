using System.ComponentModel.DataAnnotations;

namespace GetFitApp.AdminViewModel;

public class EditUserViewModel
{
    public EditUserViewModel()
    {
        Roles = new List<string>();
    }

    public string Id { get; set; } = default!;

    [Required]
    public string UserName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    public IList<string> Roles { get; set; }
}

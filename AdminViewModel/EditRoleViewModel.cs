using System.ComponentModel.DataAnnotations;

namespace GetFitApp.AdminViewModel;

public class EditRoleViewModel
{
    public EditRoleViewModel()
    {
        Users = new List<string>();
    }

    public string Id { get; set; } = default!;

    [Required(ErrorMessage = "Role name is required")]
    public string RoleName { get; set; } = default!;

    public List<string> Users { get; set; } = default!;
}

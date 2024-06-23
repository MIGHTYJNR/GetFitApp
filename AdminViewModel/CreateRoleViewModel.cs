using System.ComponentModel.DataAnnotations;

namespace GetFitApp.AdminViewModel;

public class CreateRoleViewModel
{
    [Required]
    public string RoleName { get; set; } = default!;
}

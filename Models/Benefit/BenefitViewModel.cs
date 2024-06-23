using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Benefit;

public class BenefitViewModel
{
    public Guid Id { get; set; } = default!;
    public DateTime CreatedDate = DateTime.UtcNow;
    public DateTime ModifiedDate = DateTime.UtcNow;
    public string? MembershipTypeName { get; set; }

    [Display(Name = "Benefit Name:")]
    [Required(ErrorMessage = "Benefit name is required")]
    public string Name { get; set; } = default!;

    [Display(Name = "Description:")]
    public string? Description { get; set; }

    [Display(Name = "Membership Type:")]
    [Required(ErrorMessage = "Please select a membership type")]
    public Guid MembershipTypeId { get; set; }
    public List<SelectListItem> MembershipTypes { get; set; } = []; 
}

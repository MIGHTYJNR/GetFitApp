using GetFitApp.Data.Entities;
using GetFitApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.MembershipType
{
    public class CreateMembershipTypeViewModel
    {
        public DateTime CreatedDate = DateTime.UtcNow;

        [Display(Name = "Membership Type Name:")]
        [Required(ErrorMessage = "Membership type name is required")]
        public string MembershipTypeName { get; set; } = default!;

        [Display(Name = "Duration Type:")]
        [Required(ErrorMessage = "Please select duration")]
        public DurationType Duration { get; set; } = default!;

        [Display(Name = "Price:")]
        [Required(ErrorMessage = "Amount is required")]
        public decimal Amount { get; set; }
    }
}

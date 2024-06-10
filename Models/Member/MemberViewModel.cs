using GetFitApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Member
{
    public class MemberViewModel
    {
        [Display(Name = "Firstname")]
        [Required]
        public string Firstname { get; set; } = default!;

        [Display(Name = "Lastname")]
        [Required]
        public string Lastname { get; set; } = default!;

        [Display(Name = "Middlename")]
        [Required]
        public string Middlename { get; set; } = default!;

        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Display(Name = "Phone Number")]
        [Required]
        public string PhoneNumber { get; set; } = default!;

        [Display(Name = "Age")]
        [Required]
        public int Age { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Please select a gender")]
        public Gender Gender { get; set; } = default!;

        [Display(Name = "Address")]
        [Required]
        public string Address { get; set; } = default!;

        [Display(Name = "Emergency Contact")]
        [Required]
        public string EmergencyContact { get; set; } = default!;

        [Display(Name = "Fitness Goal")]
        [Required]
        public string FitnessGoal { get; set; } = default!;

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "Please select a Membership Type")]
        public Guid MembershipTypeId { get; set; }
        public List<SelectListItem> MembershipTypes { get; set; } = new List<SelectListItem>();

        [Display(Name = "Trainer")]
        [Required(ErrorMessage = "Please select a trainer")]
        public Guid TrainerId { get; set; }
        public List<SelectListItem> Trainers { get; set; } = new List<SelectListItem>();

        [Display(Name = "Classes")]
        [Required(ErrorMessage = "Please select at least one class")]
        public Guid FitnessClassId { get; set; }
        public List<Guid> FitnessClassIds { get; set; } = new List<Guid>();

        public List<SelectListItem> FitnessClasses { get; set; } = new List<SelectListItem>();
    }
}

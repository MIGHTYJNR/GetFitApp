using GetFitApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Trainer
{
    public class TrainerViewModel
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

        [Display(Name = "Specialization")]
        [Required(ErrorMessage = "Please select area of specialization")]
        public Guid SpecializationId { get; set; }
        public List<SelectListItem> Specializations { get; set; } = new List<SelectListItem>();
    }
}

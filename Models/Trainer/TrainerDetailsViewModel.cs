using GetFitApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Trainer
{
    public class TrainerDetailsViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Firstname:")]
        public string Firstname { get; set; } = default!;

        [Display(Name = "Lastname:")]
        public string Lastname { get; set; } = default!;

        [Display(Name = "Middlename:")]
        public string Middlename { get; set; } = default!;

        [Display(Name = "Specialization:")]
        public Guid SpecializationId { get; set; } = default!;
        public string SpecializationName { get; set; } = default!;

        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Display(Name = "Phone Number:")]
        public string PhoneNumber { get; set; } = default!;

        [Display(Name = "Age:")]
        public int Age { get; set; }

        [Display(Name = "Gender:")]
        public Gender Gender { get; set; } = default!;

        [Display(Name = "Address:")]
        public string Address { get; set; } = default!;

        [Display(Name = "Fitness Goal:")]
        public string FitnessGoal { get; set; } = default!;

        [Display(Name = "Fitness Class:")]
        public string Name { get; set; } = default!;

        [Display(Name = "Class Schedule:")]
        public string Schedule { get; set; } = default!;

        [Display(Name = "Class Duration:")]
        public int Duration { get; set; }
    }
}

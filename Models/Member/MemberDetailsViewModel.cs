using GetFitApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Member
{
    public class MemberDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Firstname:")]
        public string Firstname { get; set; } = default!;

        [Display(Name = "Lastname:")]
        public string Lastname { get; set; } = default!;

        [Display(Name = "Middlename:")]
        public string Middlename { get; set; } = default!;

        [Display(Name = "Email:")]
        public string Email { get; set; } = default!;

        [Display(Name = "Phone Number:")]
        public string PhoneNumber { get; set; } = default!;

        [Display(Name = "Age:")]
        public int Age { get; set; }

        [Display(Name = "Gender:")]
        public Gender Gender { get; set; } = default!;

        [Display(Name = "Address:")]
        public string Address { get; set; } = default!;

        [Display(Name = "Emergency Contact:")]
        public string EmergencyContact { get; set; } = default!;

        [Display(Name = "Fitness Goals:")]
        public string FitnessGoal { get; set; } = default!;

        [Display(Name = "Fitness Class:")]
        public string FitnessClassName { get; set; } = default!;
        public List<Guid> FitnessClassIds { get; set; } = new List<Guid>();

        [Display(Name = "Membership Type:")]
        public Guid MembershipTypeId { get; set; } = default!;

        [Display(Name = "Preferred Trainer:")]
        public Guid TrainerId { get; set; } = default!;

        public string FitnessClassSchedule { get; set; } = default!;

        public string MembershipTypeName { get; set; } = default!;
        public string MembershipTypeBenefits { get; set; } = default!;

        public string TrainerName { get; set; } = default!;
        public string TrainerSpecialization { get; set; } = default!;
    }
}

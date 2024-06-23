using GetFitApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.MembershipType
{
    public class MembershipTypeViewModel
    {
        public Guid Id { get; set; }
        public DateTime ModifiedDate = DateTime.UtcNow;

        [Display(Name = "Membership Type Name:")]
        public string MembershipTypeName { get; set; } = default!;

        [Display(Name = "Duration Type:")]
        public DurationType Duration { get; set; } = default!;

        [Display(Name = "Price:")]
        public decimal Amount { get; set; }
    }
}

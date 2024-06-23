using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Member
{
    public class SubscriptionViewModel
    {
        public DateTime ModifiedDate = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "Please select a Membership Type")]
        public Guid MembershipTypeId { get; set; }
        public List<SelectListItem> MembershipTypes { get; set; } = new List<SelectListItem>();
    }
}

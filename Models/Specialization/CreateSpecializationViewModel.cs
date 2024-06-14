using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Specialization
{
    public class CreateSpecializationViewModel
    {
        public DateTime CreatedDate = DateTime.UtcNow;

        [Display(Name = "Specialization Name")]
        [Required(ErrorMessage = "Specialization name is required")]
        public string SpecializationName { get; set; } = default!;

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
    }
}

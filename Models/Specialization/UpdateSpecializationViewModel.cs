using GetFitApp.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Specialization
{
    public class UpdateSpecializationViewModel
    {
        public Guid Id { get; set; } = default!;

        [Required(ErrorMessage = "Specialization name is required")]
        public string SpecializationName { get; set; } = default!;

        public bool IsAvailable { get; set; }
    }
}

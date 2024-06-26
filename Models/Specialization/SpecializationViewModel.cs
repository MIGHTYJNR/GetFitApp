﻿using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Specialization
{
    public class SpecializationViewModel
    {
        public Guid Id { get; set; } = default!;
        public DateTime ModifiedDate = DateTime.UtcNow;

        [Display(Name = "Specialization Name")]
        [Required(ErrorMessage = "Specialization name is required")]
        public string SpecializationName { get; set; } = default!;

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
    }
}

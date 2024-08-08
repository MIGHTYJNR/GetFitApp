using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.FitnessClass;

public class FitnessClassViewModel
{
    public Guid Id { get; set; } = default!;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Class Name:")]
    [Required(ErrorMessage = "Fitness class name is required")]
    public string Name { get; set; } = default!;

    [Display(Name = "Class Schedule:")]
    [Required(ErrorMessage = "Class schedule is required")]
    public string Schedule { get; set; } = default!;

    [Display(Name = "Class Duration (in minutes):")]
    [Required(ErrorMessage = "Duration is required")]
    public int Duration { get; set; }

    [Display(Name = "Class Description:")]
    public string? Description { get; set; }

    [Display(Name = "Class Image:")]
    public IFormFile? Image { get; set; }

    [Display(Name = "Existing Image URL:")]
    public string? ExistingImageUrl { get; set; }

    [Display(Name = "Trainer:")]
    [Required(ErrorMessage = "Please select a trainer")]
    public Guid TrainerId { get; set; }
    public string? TrainerName { get; set; }
    public List<SelectListItem> Trainers { get; set; } = [];
}

using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Models.FitnessClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GetFitApp.Controllers;

[Authorize]
public class FitnessClassController(INotyfService notyf, GetFitContext getFitDbContext, IWebHostEnvironment webHostEnvironment) : Controller
{
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public IActionResult CreateFitnessClass()
    {
        var trainers = _getFitDbContext.Trainers
           .Include(t => t.Specialization)
           .OrderBy(t => t.Firstname)
           .Select(t => new SelectListItem
           {
               Text = t.Firstname + " " + t.Lastname.ToUpper() + " [ " + t.Specialization.SpecializationName + " ]",
               Value = t.Id.ToString()
           }).ToList();

        var viewModel = new FitnessClassViewModel
        {
            Trainers = trainers,
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFitnessClass(FitnessClassViewModel model)
    {
        if (ModelState.IsValid)
        {
            var classExists = await _getFitDbContext.FitnessClasses
                .AnyAsync(x => x.Name == model.Name || x.Schedule == model.Schedule);

            if (classExists)
            {
                _notyfService.Warning("Class already exists or scheduled time is already taken");
                return View(model);
            }

            string? imageUrl = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                imageUrl = await UploadImage(model.Image);
            }

            var fitnessClass = new FitnessClass
            {
                Name = model.Name,
                Schedule = model.Schedule,
                Duration = model.Duration,
                ImageUrl = imageUrl,
                Description = model.Description,
                TrainerId = model.TrainerId,
                CreatedDate = DateTime.UtcNow
            };

            await _getFitDbContext.FitnessClasses.AddAsync(fitnessClass);
            var result = await _getFitDbContext.SaveChangesAsync();

            if (result > 0)
            {
                _notyfService.Success("Created successfully");
                return RedirectToAction("ListClasses");
            }
        }

        _notyfService.Error("An error occurred while creating fitness class");
        return RedirectToAction("CreateFitnessClass");
    }


    private bool IsImageValid(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return false;
        }

        string[] allowedTypes = { "image/jpeg", "image/png" };

        if (!allowedTypes.Contains(image.ContentType))
        {
            return false;
        }

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        string fileExtension = Path.GetExtension(image.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return false;
        }

        return true;
    }


    private async Task<string?> UploadImage(IFormFile image)
    {
        if (!IsImageValid(image))
        {
            _notyfService.Error("Invalid image file type. Please upload a JPEG or PNG file.");
            return null;
        }

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return "/images/" + fileName;
    }


    [HttpGet]
    public IActionResult ListClasses()
    {
        var fitnessClass = _getFitDbContext.FitnessClasses
            .Include(fc => fc.Trainer)
            .OrderBy(fc => fc.Name)
            .Select(fc => new FitnessClassViewModel
            {
                Id = fc.Id,
                Name = fc.Name.ToUpper(),
                Schedule = fc.Schedule,
                Duration = fc.Duration,
                Description = fc.Description,
                TrainerName = fc.Trainer.Firstname + " " + fc.Trainer.Lastname.ToUpper() + " [ " + fc.Trainer.Specialization.SpecializationName + " ]",
                ExistingImageUrl = fc.ImageUrl
            }).ToList();

        return View(fitnessClass);
    }


    [HttpGet]
    public IActionResult SearchClass ( string searchString)
    {
         var fitnessClass = _getFitDbContext.FitnessClasses
            .Where(fc => fc.Name.Contains(searchString) || fc.Trainer.Firstname.Equals(searchString))
            .OrderBy(fc => fc.Name)
            .Select(fc => new FitnessClassViewModel
            {
                Id = fc.Id,
                Name = fc.Name.ToUpper(),
                Schedule = fc.Schedule,
                Duration = fc.Duration,
                Description = fc.Description,
                TrainerName = fc.Trainer.Firstname + " " + fc.Trainer.Lastname.ToUpper() + " [ " + fc.Trainer.Specialization.SpecializationName + " ]"
            }).ToList();

        if (fitnessClass.Count == 0)
        {
            _notyfService.Warning("Fitness class not found.");
            return RedirectToAction("ListClasses", "FitnessClass");
        }
        _notyfService.Success("Fitness class found");
        return View("ListClasses", fitnessClass);
    }


    [HttpGet]
    public IActionResult UpdateClass(Guid Id)
    {
        var fitnessClass = _getFitDbContext.FitnessClasses
        .Include(fc => fc.Trainer)
        .FirstOrDefault(fc => fc.Id == Id);

        if (fitnessClass == null)
        {
            _notyfService.Warning("Fitness class not found.");
            return RedirectToAction("ListClasses", "FitnessClass");
        }
        var trainers = _getFitDbContext.Trainers
           .Include(t => t.Specialization)
           .OrderBy(t => t.Firstname)
           .Select(t => new SelectListItem
           {
               Text = t.Firstname + " " + t.Lastname.ToUpper() + " [ " + t.Specialization.SpecializationName + " ]",
               Value = t.Id.ToString()
           }).ToList();

        var viewModel = new FitnessClassViewModel
        {
            Id = fitnessClass.Id,
            Name = fitnessClass.Name,
            Schedule = fitnessClass.Schedule,
            Duration = fitnessClass.Duration,
            Description = fitnessClass.Description,
            TrainerId = fitnessClass.TrainerId,
            Trainers = trainers,
            ExistingImageUrl = fitnessClass.ImageUrl
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateClass(FitnessClassViewModel model)
    {
        if (ModelState.IsValid)
        {
            var fitnessClass = await _getFitDbContext.FitnessClasses.FindAsync(model.Id);

            if (fitnessClass == null)
            {
                _notyfService.Error("Class not found");
                return View(model);
            }
            else
            {
                fitnessClass.Name = model.Name;
                fitnessClass.Schedule = model.Schedule;
                fitnessClass.Duration = model.Duration;
                fitnessClass.Description = model.Description;
                fitnessClass.TrainerId = model.TrainerId;
                fitnessClass.ModifiedDate = DateTime.UtcNow;

                if (model.Image != null && model.Image.Length > 0)
                {
                    string? imageUrl = await UploadImage(model.Image);
                    if (imageUrl != null)
                    {
                        fitnessClass.ImageUrl = imageUrl;
                    }
                }

                _getFitDbContext.Update(fitnessClass);
                var result = await _getFitDbContext.SaveChangesAsync();

                if (result > 0)
                {
                    _notyfService.Success("Class details updated successfully");
                    return RedirectToAction("ListClasses");
                }
                else
                {
                    _notyfService.Error("Error! update not successful");
                    return View(model);
                }
            }
        }

        _notyfService.Error("An error occurred while updating class details");
        return RedirectToAction("UpdateClass");
    }


    [HttpPost]
    public async Task<IActionResult> DeleteClass(Guid Id)
    {
        var fitnessClass = await _getFitDbContext.FitnessClasses.FindAsync(Id);

        if (fitnessClass == null)
        {
            _notyfService.Error("Class not found");
            return View();
        }

        _getFitDbContext.FitnessClasses.Remove(fitnessClass);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Deleted successfully");
            return RedirectToAction("ListClasses");
        }
        else
        {
            _notyfService.Error("An error occurred while deleting fitness class");
            return RedirectToAction("ListClasses");
        }
    }
}

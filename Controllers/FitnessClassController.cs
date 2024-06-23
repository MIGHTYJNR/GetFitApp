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
public class FitnessClassController(INotyfService notyf, GetFitContext getFitDbContext) : Controller
{
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;

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
            var classExists = await _getFitDbContext.FitnessClasses.AnyAsync(x => x.Name == model.Name || x.Schedule == model.Schedule);

            if (classExists)
            {
                _notyfService.Warning("Class already exists or scheduled time is already taken");
                return View(model);
            }

            var fitnessClass = new FitnessClass
            {
                Name = model.Name,
                Schedule = model.Schedule,
                Duration = model.Duration,
                TrainerId = model.TrainerId,
                CreatedDate = model.CreatedDate
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
                TrainerId = fc.TrainerId,
                TrainerName = fc.Trainer.Firstname + " " + fc.Trainer.Lastname.ToUpper() + " [ " + fc.Trainer.Specialization.SpecializationName + " ]",
            }).ToList();

        return View(fitnessClass);
    }


    [HttpGet]
    public IActionResult UpdateClass(Guid Id)
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
    public async Task<IActionResult> UpdateClass(FitnessClassViewModel model)
    {
        if (ModelState.IsValid)
        {
            var fitnessClass = await _getFitDbContext.FitnessClasses.FindAsync(model.Id);

            if (fitnessClass == null)
            {
                _notyfService.Error("Benefit not found");
                return View(model);
            }
            else
            {
                fitnessClass.Name = model.Name;
                fitnessClass.Schedule = model.Schedule;
                fitnessClass.Duration = model.Duration;
                fitnessClass.TrainerId = model.TrainerId;
                fitnessClass.ModifiedDate = model.ModifiedDate;

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

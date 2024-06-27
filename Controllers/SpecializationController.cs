using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data.Entities;
using GetFitApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GetFitApp.Models.Specialization;
using System.Data;
using GetFitApp.Models.Benefit;

namespace GetFitApp.Controllers;

[Authorize]
public class SpecializationController(INotyfService notyf, GetFitContext getFitDbContext) : Controller
{
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public IActionResult CreateSpecialization()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpecialization(CreateSpecializationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var specializationExists = await _getFitDbContext.Specializations.AnyAsync(x => x.SpecializationName == model.SpecializationName);

            if (specializationExists)
            {
                _notyfService.Warning("Specialization already exists");
                return View(model);
            }

            var specialization = new Specialization
            {
                SpecializationName = model.SpecializationName,
                IsAvailable = true,
                CreatedDate = model.CreatedDate
            };

            await _getFitDbContext.Specializations.AddAsync(specialization);
            var result = await _getFitDbContext.SaveChangesAsync();

            if (result > 0)
            {
                _notyfService.Success("Created successfully");
                return RedirectToAction("ListSpecialization", "Specialization");
            }
        }

        _notyfService.Error("An error occurred while creating specialization");
        return View(model);
    }


    [HttpGet]
    public IActionResult ListSpecialization()
    {
        var specializations = _getFitDbContext.Specializations
            .OrderBy(s => s.SpecializationName)
            .Select(s => new SpecializationViewModel
        {
            Id = s.Id,
            SpecializationName = s.SpecializationName,
            IsAvailable = s.IsAvailable
        }).ToList();

        return View(specializations);
    }


    [HttpGet]
    public IActionResult SearchSpecialization(string searchString)
    {
        var specializations = _getFitDbContext.Specializations
           .Where(s => s.SpecializationName.Contains(searchString))
            .OrderBy(s => s.SpecializationName)
            .Select(s => new SpecializationViewModel
            {
                Id = s.Id,
                SpecializationName = s.SpecializationName,
                IsAvailable = s.IsAvailable
            })
            .ToList();

        if (specializations.Count == 0)
        {
            _notyfService.Warning("Specialization not found.");
            return RedirectToAction("ListSpecialization", "Specialization");
        }
        _notyfService.Success("Specialization found");
        return View("ListSpecialization", specializations);
    }


    [HttpGet]
    public async Task<IActionResult> UpdateSpecialization(Guid Id)
    {
        var specialization = await _getFitDbContext.Specializations.FindAsync(Id);

        if (specialization == null)
        {
            _notyfService.Error("Specialization not found");
            return View();
        }

        var model = new SpecializationViewModel 
        {
            Id = specialization.Id,
            SpecializationName = specialization.SpecializationName,
            IsAvailable = specialization.IsAvailable
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateSpecialization(SpecializationViewModel model)
    {
        if (ModelState.IsValid)
        {
            var specialization = await _getFitDbContext.Specializations.FindAsync(model.Id);

            if (specialization == null)
            {
                _notyfService.Warning("Specialization not found");
                return View();
            }
            else
            {
                specialization.SpecializationName = model.SpecializationName;
                specialization.IsAvailable = model.IsAvailable;
                specialization.ModifiedDate = model.ModifiedDate;

                _getFitDbContext.Update(specialization);
                var result = await _getFitDbContext.SaveChangesAsync();

                if (result > 0)
                {
                    _notyfService.Success("Updated successfully");
                    return RedirectToAction("ListSpecialization");
                }
                else
                {
                    _notyfService.Error("Error! update not successful");
                    return View(model);
                }
            }
        }

        _notyfService.Error("An error occurred while updating specialization");
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteSpecialization(Guid Id)
    {
        var specialization = await _getFitDbContext.Specializations.FindAsync(Id);

        if (specialization == null)
        {
            _notyfService.Error("Specialization not found");
            return View();
        }

        _getFitDbContext.Specializations.Remove(specialization);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Deleted successfully");
            return RedirectToAction("ListSpecialization");
        }
        else
        {
            _notyfService.Error("An error occurred while deleting specialization");
            return RedirectToAction("ListSpecialization");
        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data.Entities;
using GetFitApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GetFitApp.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GetFitApp.Models.Trainer;

namespace GetFitApp.Controllers;

public class TrainerController(UserManager<User> userManager,
    INotyfService notyf,
    GetFitContext getFitDbContext,
    IHttpContextAccessor httpContextAccessor) : Controller
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult TrainerRegistration()
    {
        var specializations = _getFitDbContext.Specializations
            .OrderBy(s => s.SpecializationName)
            .Where(s => s.IsAvailable)
            .Select(s => new SelectListItem
        {
            Text = s.SpecializationName,
            Value = s.Id.ToString()
        }).ToList();

        var viewModel = new TrainerViewModel
        {
            Specializations = specializations
        };

        return View(viewModel);
    }

    [HttpPost("Trainer/TrainerRegistration")]
    public async Task<IActionResult> TrainerRegistration(TrainerViewModel model)
    {
        var userDetail = await Helper.GetCurrentUserIdAsync(_httpContextAccessor, _userManager);
        var trainerExist = await _getFitDbContext.Trainers.AnyAsync(x => x.UserId == userDetail.userId);

        if (trainerExist)
        {
            _notyfService.Warning("Trainer already exists");
            return View(model);
        }

        var trainer = new Trainer
        {
            UserId = userDetail.userId,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Middlename = model.Middlename,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Age = model.Age,
            Gender = model.Gender,
            Address = model.Address,
            SpecializationId = model.SpecializationId,
            CreatedDate = model.CreatedDate
        };

        await _getFitDbContext.AddAsync(trainer);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Registration successful");
            return RedirectToAction("Index", "Trainer");
        }

        _notyfService.Error("An error occurred while creating trainer");
        return View();
    }


    [HttpGet("Trainer/ViewTrainerDetails")]
    public async Task<IActionResult> ViewTrainerDetails()
    {
        var user = await _userManager.GetUserAsync(User);

        var trainer = await _getFitDbContext.Trainers
            .Include(t => t.Specialization)
            .FirstOrDefaultAsync(t => t.UserId == user!.Id);

        if (trainer != null)
        {
            var trainerDetailsViewModel = new TrainerDetailsViewModel
            {
                Firstname = trainer.Firstname.ToUpper(),
                Lastname = trainer.Lastname.ToUpper(),
                Middlename = trainer.Middlename,
                SpecializationId = trainer.SpecializationId,
                Email = trainer.Email,
                PhoneNumber = trainer.PhoneNumber,
                Age = trainer.Age,
                Gender = trainer.Gender,
                Address = trainer.Address,

                SpecializationName = trainer.Specialization.SpecializationName.ToUpper(),
            };

            return View(trainerDetailsViewModel);
        }
        else
        {
            _notyfService.Error("Details not found");
            return RedirectToAction("TrainerRegistration", "Trainer");
        }
    }


    public IActionResult UpdateTrainerDetails()
    {
        var specializations = _getFitDbContext.Specializations.Select(s => new SelectListItem
        {
            Text = s.SpecializationName,
            Value = s.Id.ToString()
        }).ToList();

        var viewModel = new TrainerViewModel
        {
            Specializations = specializations
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTrainerDetails(TrainerViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var trainer = await _getFitDbContext.Trainers
                .FirstOrDefaultAsync(t => t.UserId == user!.Id);

            if (trainer != null)
            {
                trainer.Firstname = model.Firstname;
                trainer.Lastname = model.Lastname;
                trainer.Middlename = model.Middlename;
                trainer.Email = model.Email;
                trainer.PhoneNumber = model.PhoneNumber;
                trainer.Age = model.Age;
                trainer.Gender = model.Gender;
                trainer.Address = model.Address;
                trainer.SpecializationId = model.SpecializationId;
                trainer.ModifiedDate = model.ModifiedDate;

                _getFitDbContext.Update(trainer);
                await _getFitDbContext.SaveChangesAsync();

                _notyfService.Success("Details updated successfully");
                return RedirectToAction("Index", "Trainer");
            }
            else
            {
                _notyfService.Error("Details not found");
                return RedirectToAction("TrainerRegistration", "Trainer");
            }
        }

        _notyfService.Error("An error occurred while updating detail");
        return View(model);
    }


    public IActionResult DeleteTrainer()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTrainer(TrainerDetailsViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        var trainer = await _getFitDbContext.Trainers
            .FirstOrDefaultAsync(t => t.UserId == user!.Id);

        if (trainer != null)
        {
            _getFitDbContext.Remove(trainer);
            await _getFitDbContext.SaveChangesAsync();

            _notyfService.Success("Details deleted successfully");
            return RedirectToAction("TrainerRegistration", "Trainer");
        }
        else
        {
            _notyfService.Error("Details not found");
            return RedirectToAction("TrainerRegistration", "Trainer");
        }
    }
}


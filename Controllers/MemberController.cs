using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Models.Member;
using GetFitApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GetFitApp.Controllers;

[Authorize]
public class MemberController(UserManager<User> userManager,
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


    public IActionResult MemberRegistration()
    {
        var membershipTypes = _getFitDbContext.MembershipTypes.Select(mt => new SelectListItem
        {
            Text = mt.MembershipTypeName,
            Value = mt.Id.ToString()
        }).ToList();

        var trainers = _getFitDbContext.Trainers.Select(t => new SelectListItem
        {
            Text = t.Firstname,
            Value = t.Id.ToString()
        }).ToList();

        var fitnessClasses = _getFitDbContext.FitnessClasses.Select(fc => new SelectListItem
        {
            Text = fc.Name,
            Value = fc.Id.ToString()
        }).ToList();

        var viewModel = new MemberViewModel
        {
            MembershipTypes = membershipTypes,
            Trainers = trainers,
            FitnessClasses = fitnessClasses
        };

        return View(viewModel);
    }

    [HttpPost("Member/MemberRegistration")]
    public async Task<IActionResult> MemberRegistration(MemberViewModel model)
    {
        var memberExist = await _getFitDbContext.MemberDetails.AnyAsync(x => x.PhoneNumber == model.PhoneNumber || x.Email == model.Email);

        var userDetail = await Helper.GetCurrentUserIdAsync(_httpContextAccessor, _userManager);

        if (memberExist)
        {
            _notyfService.Warning("Member already exists");
            return View(model);
        }

        var member = new Member
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
            EmergencyContact = model.EmergencyContact,
            FitnessGoal = model.FitnessGoal,
            MembershipTypeId = model.MembershipTypeId,
            TrainerId = model.TrainerId,
            FitnessClassId = model.FitnessClassId
        };

        await _getFitDbContext.AddAsync(member);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Member registered successfully");
            return RedirectToAction("Index", "Member");
        }

        _notyfService.Error("An error occurred while creating membership");
        return View();
    }


    [HttpGet("Member/ViewMemberDetails")]
    public async Task<IActionResult> ViewMemberDetails()
    {
        var user = await _userManager.GetUserAsync(User);

        var member = await _getFitDbContext.MemberDetails
            .Include(m => m.MembershipType)
            .Include(m => m.PreferredTrainer)
            .Include(m => m.FitnessClass)
            .FirstOrDefaultAsync(m => m.UserId == user!.Id);

        if (member != null)
        {
            var memberDetailsViewModel = new MemberDetailsViewModel
            {
                Firstname = member.Firstname,
                Lastname = member.Lastname,
                Middlename = member.Middlename,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                Age = member.Age,
                Gender = member.Gender,
                Address = member.Address,
                EmergencyContact = member.EmergencyContact,
                FitnessGoal = member.FitnessGoal,
                MembershipTypeId = member.MembershipTypeId,
                TrainerId = member.TrainerId,
                FitnessClassName = member.FitnessClass.Name.ToUpper(),

                FitnessClassSchedule = member.FitnessClass.Schedule,
                MembershipTypeName = member.MembershipType.MembershipTypeName.ToUpper(),
                MembershipTypeBenefits = member.MembershipType.Benefits.ToString()!,
                TrainerName = member.PreferredTrainer.Firstname.ToUpper(),
                TrainerSpecialization = member.PreferredTrainer.SpecializationId.ToString(),

            };

            return View(memberDetailsViewModel);
        }
        else
        {
            _notyfService.Error("Member details not found");
            return RedirectToAction("MemberRegistration", "Member");
        }
    }


    public IActionResult UpdateMemberDetails()
    {
        var membershipTypes = _getFitDbContext.MembershipTypes.Select(mt => new SelectListItem
        {
            Text = mt.MembershipTypeName,
            Value = mt.Id.ToString()
        }).ToList();

        var trainers = _getFitDbContext.Trainers.Select(t => new SelectListItem
        {
            Text = t.Firstname,
            Value = t.Id.ToString()
        }).ToList();

        var fitnessClasses = _getFitDbContext.FitnessClasses.Select(fc => new SelectListItem
        {
            Text = fc.Name,
            Value = fc.Id.ToString()
        }).ToList();

        var ViewModel = new MemberViewModel
        {
            MembershipTypes = membershipTypes,
            Trainers = trainers,
            FitnessClasses = fitnessClasses
        };

        return View(ViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMemberDetails(MemberViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var member = await _getFitDbContext.MemberDetails
                .FirstOrDefaultAsync(m => m.UserId == user!.Id);

            if (member != null)
            {
                member.Firstname = model.Firstname;
                member.Lastname = model.Lastname;
                member.Middlename = model.Middlename;
                member.Email = model.Email;
                member.PhoneNumber = model.PhoneNumber;
                member.Age = model.Age;
                member.Gender = model.Gender;
                member.Address = model.Address;
                member.EmergencyContact = model.EmergencyContact;
                member.FitnessGoal = model.FitnessGoal;
                member.MembershipTypeId = model.MembershipTypeId;
                member.TrainerId = model.TrainerId;
                member.FitnessClassId = model.FitnessClassId;

                _getFitDbContext.Update(member);
                await _getFitDbContext.SaveChangesAsync();

                _notyfService.Success("Member details updated successfully");
                return RedirectToAction("Index", "Member");
            }
            else
            {
                _notyfService.Error("Member details not found");
                return RedirectToAction("MemberRegistration", "Member");
            }
        }

        _notyfService.Error("An error occurred while updating detail");
        return View(model);
    }


    public IActionResult DeleteMember()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMember(MemberDetailsViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        var member = await _getFitDbContext.MemberDetails
            .FirstOrDefaultAsync(m => m.UserId == user!.Id);

        if (member != null)
        {
            _getFitDbContext.Remove(member);
            await _getFitDbContext.SaveChangesAsync();

            _notyfService.Success("Details deleted successfully");
            return RedirectToAction("MemberRegistration", "Member");
        }
        else
        {
            _notyfService.Error("Member details not found");
            return RedirectToAction("MemberRegistration", "Member");
        }
    }

}

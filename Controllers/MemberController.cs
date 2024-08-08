using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Data.Enums;
using GetFitApp.Models.FitnessClass;
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
        var fitnessClass = _getFitDbContext.FitnessClasses
            .Select(fc => new FitnessClassDisplayViewModel
            {
                Name = fc.Name,
                Description = fc.Description,
                Schedule = fc.Schedule,
                ImageUrl = fc.ImageUrl
            }).ToList();

        return View(fitnessClass);
    }


    public IActionResult MemberRegistration()
    {
        var membershipTypes = _getFitDbContext.MembershipTypes
            .Include(mt => mt.Benefits)
            .OrderBy(mt => mt.Duration)
            .Select(mt => new SelectListItem
        {
            Text = mt.MembershipTypeName.ToUpper() + " [" + "Subscription: " + mt.Duration.ToString() + " | " + "Price: " + mt.Amount + " | " + "Benefits: " + string.Join(",", mt.Benefits.Select(b => b.Description)) + "]",
            Value = mt.Id.ToString()
            }).ToList();

        var trainers = _getFitDbContext.Trainers
            .Include(t => t.Specialization)
            .OrderBy(t => t.Firstname)
            .Select(t => new SelectListItem
        {
            Text = t.Firstname + " " + t.Lastname.ToUpper() + " [ " + t.Specialization.SpecializationName + " ] ",
            Value = t.Id.ToString()
        }).ToList();

        var fitnessClasses = _getFitDbContext.FitnessClasses
            .OrderBy(fc => fc.Schedule)
            .Select(fc => new SelectListItem
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

        var selectedMembershipType = await _getFitDbContext.MembershipTypes.FirstOrDefaultAsync(mt => mt.Id == model.MembershipTypeId);

        if (selectedMembershipType == null)
        {
            _notyfService.Error("Invalid membership type selected");
            return View(model);
        }

        var expirationDate = model.CreatedDate;

        switch (selectedMembershipType.Duration)
        {
            case DurationType.Weekly:
                expirationDate = expirationDate.AddDays(7);
                break;
            case DurationType.Monthly:
                expirationDate = expirationDate.AddMonths(1);
                break;
            case DurationType.Yearly:
                expirationDate = expirationDate.AddYears(1);
                break;
            default:
                _notyfService.Error("Invalid duration type");
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
            FitnessClassId = model.FitnessClassId,
            CreatedDate = model.CreatedDate,
            ExpiryDate = expirationDate
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
            .ThenInclude(mt => mt.Benefits)
            .Include(m => m.PreferredTrainer)
            .ThenInclude(t => t.Specialization)
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
                ExpiryDate = member.ExpiryDate,

                FitnessClassSchedule = member.FitnessClass.Schedule,
                MembershipTypeName = member.MembershipType.MembershipTypeName.ToUpper(),
                TrainerName = member.PreferredTrainer?.Firstname + " " + member.PreferredTrainer?.Lastname.ToUpper(),
                TrainerSpecialization = member.PreferredTrainer?.Specialization.SpecializationName
            };

            return View(memberDetailsViewModel);
        }
        else
        {
            _notyfService.Error("Member details not found");
            return RedirectToAction("MemberRegistration", "Member");
        }
    }


    public async Task<IActionResult> UpdateMemberDetails()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            _notyfService.Error("User not found");
            return RedirectToAction("Index", "Home");
        }

        var member = await _getFitDbContext.MemberDetails
            .Include(m => m.PreferredTrainer)
            .Include(m => m.FitnessClass)
            .FirstOrDefaultAsync(m => m.UserId == user.Id);

        if (member == null)
        {
            _notyfService.Error("Member details not found");
            return RedirectToAction("MemberRegistration", "Member");
        }

        var trainers = await _getFitDbContext.Trainers
        .Include(t => t.Specialization)
        .OrderBy(t => t.Firstname)
        .Select(t => new SelectListItem
        {
            Text = t.Firstname + " " + t.Lastname.ToUpper() + " [ " + t.Specialization.SpecializationName + " ] ",
            Value = t.Id.ToString(),
            Selected = t.Id == member.TrainerId
        }).ToListAsync();

        var fitnessClasses = await _getFitDbContext.FitnessClasses
        .OrderBy(fc => fc.Schedule)
        .Select(fc => new SelectListItem
        {
            Text = fc.Name,
            Value = fc.Id.ToString(),
            Selected = fc.Id == member.FitnessClassId
        }).ToListAsync();

        var ViewModel = new MemberViewModel
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
            TrainerId = member.TrainerId,
            FitnessClassId = member.FitnessClassId,
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
            if (user == null)
            {
                _notyfService.Error("User not found");
                return RedirectToAction("Index", "Home");
            }

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
                member.TrainerId = model.TrainerId;
                member.FitnessClassId = model.FitnessClassId;
                member.ModifiedDate = model.ModifiedDate;

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


    public IActionResult Subscription()
    {
        var membershipTypes = _getFitDbContext.MembershipTypes
        .Include(mt => mt.Benefits)
        .OrderBy(mt => mt.Duration)
        .Select(mt => new SelectListItem
        {
            Text = mt.MembershipTypeName.ToUpper() + " [" + "Subscription: " + mt.Duration.ToString() + " | " + "Price: " + mt.Amount + "]",
            Value = mt.Id.ToString()
        }).ToList();

        var ViewModel = new SubscriptionViewModel
        {
            MembershipTypes = membershipTypes
        };

        return View(ViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Subscription(SubscriptionViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
        {
            _notyfService.Error("User not found");
            return RedirectToAction("Index", "Home");
        }

            var member = await _getFitDbContext.MemberDetails
                .FirstOrDefaultAsync(m => m.UserId == user!.Id);

            if (member != null)
            {
                var selectedMembershipType = await _getFitDbContext.MembershipTypes.FirstOrDefaultAsync(mt => mt.Id == model.MembershipTypeId);

                if (selectedMembershipType == null)
                {
                    _notyfService.Error("Invalid membership type selected");
                    return View(model);
                }

                var expirationDate = model.ModifiedDate;

                switch (selectedMembershipType.Duration)
                {
                    case DurationType.Weekly:
                        expirationDate = expirationDate.AddDays(7);
                        break;
                    case DurationType.Monthly:
                        expirationDate = expirationDate.AddMonths(1);
                        break;
                    case DurationType.Yearly:
                        expirationDate = expirationDate.AddYears(1);
                        break;
                    default:
                        _notyfService.Error("Invalid duration type");
                        return View(model);
                }

                member.MembershipTypeId = model.MembershipTypeId;
                member.ModifiedDate = model.ModifiedDate;
                member.ExpiryDate = expirationDate;

                _getFitDbContext.Update(member);
                await _getFitDbContext.SaveChangesAsync();

                _notyfService.Success("Subscription successfully");
                return RedirectToAction("Index", "Member");
            }
            else
            {
                _notyfService.Error("Member not found");
                return RedirectToAction("MemberRegistration", "Member");
            }
        }

        _notyfService.Error("An error occurred while renewing subscription");
        return View(model);
    }
}

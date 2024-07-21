using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.AdminViewModel;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Models.Member;
using GetFitApp.Models.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GetFitApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdministrationController(RoleManager<IdentityRole> roleManager,
    UserManager<User> userManager,
    INotyfService notyf,
    GetFitContext getFitDbContext) : Controller
{
    private readonly RoleManager<IdentityRole> roleManager = roleManager;
    private readonly UserManager<User> userManager = userManager;
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;


    [HttpGet]
    public IActionResult ListUsers()
    {
        var users = userManager.Users
            .OrderBy(u => u.UserName);
        return View(users);
    }


    [HttpGet]
    public IActionResult SearchUser(string searchString)
    {
        var users = userManager.Users
            .Where(u => u.UserName!.Contains(searchString) || u.Email!.Equals(searchString))
            .OrderBy(u => u.UserName);

        if (!users.Any())
        {
            _notyfService.Warning("User not found.");
            return RedirectToAction("ListUsers", "Administration");
        }
        else
        {
            _notyfService.Success("Found user");
            return View("ListUsers", users);
        }
        
    }


    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            _notyfService.Warning($"User not found");
            ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
            return View("NotFound");
        }

        var userRoles = await userManager.GetRolesAsync(user);

        var model = new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            Roles = userRoles
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        var user = await userManager.FindByIdAsync(model.Id);

        if (user == null)
        {
            _notyfService.Warning($"User not found");
            ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
            return View("NotFound");
        }
        else
        {
            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _notyfService.Success("User details updated successfully");
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            _notyfService.Error("An error occurred while updating detail");
            return View(model);
        }
    }


    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);

        if (user == null)
        {
            _notyfService.Warning($"User not found");
            ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
            return View("NotFound");
        }
        else
        {
            var member = await _getFitDbContext.MemberDetails.FirstOrDefaultAsync(m => m.UserId == id);

            if (member != null)
            {
                _getFitDbContext.MemberDetails.Remove(member);
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _notyfService.Success("User deleted successfully");
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            _notyfService.Error("An error occurred while deleting user");
            return View("ListUsers");
        }
    }


    [HttpGet]
    public IActionResult ListAllTrainers()
    {
        var trainers = _getFitDbContext.Trainers
            .OrderBy(t => t.Firstname)
            .Select(t => new TrainerDetailsViewModel
            {
                Id = t.Id,
                Firstname = t.Firstname,
                Lastname = t.Lastname,
                Middlename = t.Middlename,
                Email = t.Email,
                PhoneNumber = t.PhoneNumber,
                Age = t.Age,
                Gender = t.Gender,
                Address = t.Address,
                SpecializationName = t.Specialization.SpecializationName.ToUpper(),
            }).ToList();

        return View(trainers);
    }



    [HttpGet]
    public IActionResult SearchTrainer(string searchString)
    {
        var trainers = _getFitDbContext.Trainers
            .Where(t => t.Firstname.Contains(searchString) || t.Lastname.Contains(searchString) || t.Email.Equals(searchString))
            .OrderBy(t => t.Firstname)
            .Select(t => new TrainerDetailsViewModel
            {
                Id = t.Id,
                Firstname = t.Firstname,
                Lastname = t.Lastname,
                Middlename = t.Middlename,
                Email = t.Email,
                PhoneNumber = t.PhoneNumber,
                Age = t.Age,
                Gender = t.Gender,
                Address = t.Address,
                SpecializationName = t.Specialization.SpecializationName.ToUpper(),
            }).ToList();
        if (!trainers.Any())
        {
            _notyfService.Warning("Trainer not found.");
            return RedirectToAction("ListAllTrainers", "Administration");
        }
        else
        {
            _notyfService.Success("Successful");
            return View("ListAllTrainers", trainers);
        }
    }


    [HttpGet]
    public IActionResult EditTrainer(Guid Id)
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
    public async Task<IActionResult> EditTrainer(TrainerViewModel model)
    {
        if (ModelState.IsValid)
        {
            var trainer = await _getFitDbContext.Trainers.FindAsync(model.Id);

            if (trainer == null)
            {
                _notyfService.Error("Trainer not found");
                return View(model);
            }
            else

            {
                trainer.Firstname = model.Firstname;
                trainer.Lastname = model.Lastname;
                trainer.Middlename = model.Middlename;
                trainer.Email = model.Email;
                trainer.PhoneNumber = model.PhoneNumber;
                trainer.Age = model.Age;
                trainer.Gender = model.Gender;
                trainer.Address = model.Address;
                

                _getFitDbContext.Update(trainer);
                var result = await _getFitDbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _notyfService.Success("Trainer details updated successfully");
                    return RedirectToAction("ListAllTrainers");
                }
                else
                {
                    _notyfService.Error("Error! update not successful");
                    return View(model);
                }
            }
        }
        _notyfService.Error("An error occurred while updating trainer details");
        return RedirectToAction("EditTrainer");

    }


    [HttpPost]
    public async Task<IActionResult> DeleteTrainer(Guid Id)
    {
        var trainer = await _getFitDbContext.Trainers.FindAsync(Id);

        if (trainer == null)
        {
            _notyfService.Error("Trainer not found");
            return View();
        }

        _getFitDbContext.Trainers.Remove(trainer);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Deleted successfully");
            return RedirectToAction("ListAllTrainers");
        }
        else
        {
            _notyfService.Error("An error occurred while deleting trainer details");
            return RedirectToAction("ListAllTrainers");
        }
    }


    [HttpGet]
    public IActionResult ListAllMembers()
    {
        var members = _getFitDbContext.MemberDetails
            .OrderBy(m => m.Firstname)
            .Select(m => new MemberDetailsViewModel
            {
                Id = m.Id,
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                Middlename = m.Middlename,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                Age = m.Age,
                Gender = m.Gender,
                Address = m.Address,
                EmergencyContact = m.EmergencyContact,
                FitnessGoal = m.FitnessGoal,
                FitnessClassName = m.FitnessClass.Name,
                MembershipTypeName = m.MembershipType.MembershipTypeName,
                TrainerName = m.PreferredTrainer.Firstname,
                ExpiryDate = m.ExpiryDate
            }).ToList();

        return View(members);
    }


    [HttpGet]
    public IActionResult SearchMember(string searchString)
    {
        var members = _getFitDbContext.MemberDetails
            .Where(m => m.Firstname.Contains(searchString) || m.Lastname.Contains(searchString) || m.Email.Equals(searchString))
            .OrderBy(m => m.Firstname)
            .Select(m => new MemberDetailsViewModel
            {
                Id = m.Id,
                Firstname = m.Firstname,
                Lastname = m.Lastname,
                Middlename = m.Middlename,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                Age = m.Age,
                Gender = m.Gender,
                Address = m.Address,
                EmergencyContact = m.EmergencyContact,
                FitnessGoal = m.FitnessGoal,
                FitnessClassName = m.FitnessClass.Name,
                MembershipTypeName = m.MembershipType.MembershipTypeName,
                TrainerName = m.PreferredTrainer.Firstname,
                ExpiryDate = m.ExpiryDate
            }).ToList();
        if (!members.Any())
        {
            _notyfService.Warning("Member not found.");
            return RedirectToAction("ListAllMembers", "Administration");
        }
        else
        {
            _notyfService.Success("Successful");
            return View("ListAllMembers", members);
        }
    }


    [HttpGet]
    public IActionResult EditMember(Guid Id)
    {
        var trainers = _getFitDbContext.Trainers
         .Include(t => t.Specialization)
         .OrderBy(t => t.Firstname)
         .Select(t => new SelectListItem
         {
             Text = t.Firstname + " " + t.Lastname.ToUpper() + " [ " + t.Specialization.SpecializationName + " ] ",
             Value = t.Id.ToString()
         }).ToList();

        var fitnessClasses = _getFitDbContext.FitnessClasses.Select(fc => new SelectListItem
        {
            Text = fc.Name,
            Value = fc.Id.ToString()
        }).ToList();

        var viewModel = new MemberViewModel
        {
            Trainers = trainers,
            FitnessClasses = fitnessClasses  
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditMember(MemberViewModel model)
    {
        if (ModelState.IsValid)
        {
            var member = await _getFitDbContext.MemberDetails.FindAsync(model.Id);

            if (member == null)
            {
                _notyfService.Error("Member not found");
                return View(model);
            }
            else

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
                var result = await _getFitDbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _notyfService.Success("Member details updated successfully");
                    return RedirectToAction("ListAllMembers");
                }
                else
                {
                    _notyfService.Error("Error! update not successful");
                    return View(model);
                }
            }
        }
        _notyfService.Error("An error occurred while updating member details");
        return RedirectToAction("EditMember");

    }


    [HttpPost]
    public async Task<IActionResult> DeleteMember(Guid Id)
    {
        var member = await _getFitDbContext.MemberDetails.FindAsync(Id);

        if (member == null)
        {
            _notyfService.Error("Member not found");
            return View();
        }

        _getFitDbContext.MemberDetails.Remove(member);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Deleted successfully");
            return RedirectToAction("ListAllMembers");
        }
        else
        {
            _notyfService.Error("An error occurred while deleting member details");
            return RedirectToAction("ListAllMembers");
        }
    }


    [HttpGet]
    public async Task<IActionResult> ManageUserRoles(string userId)
    {
        ViewBag.userId = userId;

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _notyfService.Warning("User not found");
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return View("NotFound");
        }

        var roles = await roleManager.Roles.ToListAsync(); // Load roles into memory

        var model = new List<ManageUserRolesViewModel>();

        foreach (var role in roles)
        {
            var manageUserRolesViewModel = new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name!
            };

            if (await userManager.IsInRoleAsync(user, role.Name!))
            {
                manageUserRolesViewModel.IsSelected = true;
            }
            else
            {
                manageUserRolesViewModel.IsSelected = false;
            }

            model.Add(manageUserRolesViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageUserRoles(List<ManageUserRolesViewModel> model, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _notyfService.Warning("User not found");
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return View("NotFound");
        }

        var roles = await userManager.GetRolesAsync(user);
        var result = await userManager.RemoveFromRolesAsync(user, roles);

        if (!result.Succeeded)
        {
            _notyfService.Error("Cannot remove user existing roles");
            return View(model);
        }

        result = await userManager.AddToRolesAsync(user,
            model.Where(x => x.IsSelected).Select(y => y.RoleName));

        if (!result.Succeeded)
        {
            _notyfService.Error("Cannot add selected roles to user");
            return View(model);
        }

        return RedirectToAction("EditUser", new { Id = userId });
    }


    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            IdentityRole identityRole = new()
            {
                Name = model.RoleName
            };
            IdentityResult result = await roleManager.CreateAsync(identityRole);

            if (result.Succeeded)
            {
                _notyfService.Success("Role created successfully");
                return RedirectToAction("ListRoles", "Administration");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        _notyfService.Error("An error occurred while creating role");
        return View(model);
    }


    [HttpGet]
    public IActionResult ListRoles()
    {
        var roles = roleManager.Roles;
        return View(roles);
    }


    [HttpGet]
    public IActionResult SearchRole(string searchString)
    {
        var roles = roleManager.Roles
            .Where(r => r.Name!.Contains(searchString));

        if (!roles.Any())
        {
            _notyfService.Warning("Role not found.");
            return RedirectToAction("ListRoles", "Administration");
        }
        else
        {
            _notyfService.Success("Found role");
            return View("ListRoles", roles);
        }
        
    }


    [HttpGet]
    public async Task<IActionResult> EditRole(string id)
    {
        var role = await roleManager.FindByIdAsync(id);


        if (role == null)
        {
            _notyfService.Warning("Role not found");
            ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
            return View("NotFound");
        }

        var model = new EditRoleViewModel
        {
            Id = role.Id,
            RoleName = role.Name!
        };

        var users = userManager.Users.ToList();

        foreach (var user in users)
        {
            if (await userManager.IsInRoleAsync(user, role.Name!))
            {
                model.Users.Add(user.UserName!);
            };
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditRole(EditRoleViewModel model)
    {
        var role = await roleManager.FindByIdAsync(model.Id);


        if (role == null)
        {
            _notyfService.Warning("Role not found");
            ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
            return View("NotFound");
        }
        else
        {
            role.Name = model.RoleName;
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                _notyfService.Success("Successful");
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            _notyfService.Error("An error occurred while editing role");
            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> EditUsersInRole(string roleId)
    {
        ViewBag.roleId = roleId;

        var role = await roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            _notyfService.Warning("Role not found");
            ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
            return View("NotFound");
        }

        var model = new List<UserRoleViewModel>();
        var users = userManager.Users.ToList();

        foreach (var user in users)
        {
            var userRoleViewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!
            };

            if (await userManager.IsInRoleAsync(user, role.Name!))
            {
                userRoleViewModel.IsSelected = true;
            }
            else
            {
                userRoleViewModel.IsSelected = false;
            }

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            _notyfService.Warning("Role not found");
            ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
            return View("NotFound");
        }

        for (int i = 0; i < model.Count; i++)
        {
            var user = await userManager.FindByIdAsync(model[i].UserId);

            IdentityResult result = null!;

            if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user!, role.Name!)))
            {
                result = await userManager.AddToRoleAsync(user!, role.Name!);
            }
            else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user!, role.Name!))
            {
                result = await userManager.RemoveFromRoleAsync(user!, role.Name!);
            }
            else
            {
                continue;
            }

            if (result.Succeeded)
            {
                if (i < (model.Count - 1))
                {
                    _notyfService.Success("Successful");
                    continue;
                }
                else
                    return RedirectToAction("EditRole", new { Id = roleId });
            }

        }
        return RedirectToAction("EditRole", new { Id = roleId });
    }


    public async Task<IActionResult> DeleteRole(string Id)
    {
        var role = await roleManager.FindByIdAsync(Id);

        if (role == null)
        {
            _notyfService.Warning("Role not found");
            ViewBag.ErrorMessage = $"Role with Id = {Id} cannot be found";
            return View("NotFound");
        }
        else
        {
            var result = await roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                _notyfService.Success("Role deleted successfully");
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            _notyfService.Error("An error occurred while deleting user");
            return View("ListRoles");
        }

    }
}

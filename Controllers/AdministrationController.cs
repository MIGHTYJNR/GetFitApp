using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.AdminViewModel;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Models.Benefit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetFitApp.Controllers;

//[Authorize(Roles = "Admin")]
[Authorize]
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

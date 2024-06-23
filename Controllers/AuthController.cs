using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data.Enums;
using GetFitApp.Data;
using GetFitApp.Utility;
using GetFitWebApp.ActionFilter;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GetFitApp.Data.Entities;
using GetFitApp.Models.Auth;
using AspNetCoreHero.ToastNotification.Notyf;


namespace GetFitApp.Controllers;

public class AuthController(
UserManager<User> userManager,
SignInManager<User> signInManager,
INotyfService notyf,
GetFitContext getFitDbContext,
IHttpContextAccessor httpContextAccessor) : Controller
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


    [RedirectAuthenticatedUsers]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var userType = user.UserType;
                    var userDetails = await Helper.GetCurrentUserIdAsync(_httpContextAccessor, _userManager);
                    var memberDetail = await _getFitDbContext.MemberDetails.AnyAsync(x => x.UserId == userDetails.userId);
                    var trainerDetail = await _getFitDbContext.Trainers.AnyAsync(x => x.UserId == userDetails.userId);

                    var redirectResult = memberDetail ? RedirectToAction("Index", "Member") : RedirectToAction("MemberRegistration", "Member");
                    var redirectTrainerResult = trainerDetail ? RedirectToAction("Index", "Trainer") : RedirectToAction("TrainerRegistration", "Trainer");

                    if (userType == UserType.Member)
                    {
                        var memberDetails = await _getFitDbContext.MemberDetails.FirstOrDefaultAsync(x => x.UserId == userDetails.userId);

                        if (memberDetails != null)
                        {
                            if (memberDetails.ExpiryDate < DateTime.Now)
                            {
                                _notyfService.Warning("Your subscription has expired '~'");
                                return RedirectToAction("Subscription", "Member");
                            }
                        }
                        _notyfService.Success("Login succesful");
                        return redirectResult;
                    }
                    else if (userType == UserType.Trainer)
                    {
                        _notyfService.Success("Login succesful");
                        return redirectTrainerResult;
                    }
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            _notyfService.Error("Invalid username or password");
            return View(model);
        }
        return View();
    }


    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost("/Auth/SignUp")]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email || u.UserName == model.Username);

            if (existingUser != null)
            {
                _notyfService.Warning("User already exist!");
                return View();
            }

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                UserType = model.UserType
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                _notyfService.Error("An error occured while registering user!");
                return View();
            }
            /*var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("Trainer"))
            {
                _notyfService.Success("AdminRegistration was successful");
                return RedirectToAction("ListUsers", "Administration");
            }*/
            _notyfService.Success("Registration was successful");
            //await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Login", "Auth");
        }

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _notyfService.Information("Logout was successful");
        return RedirectToAction("Login", "Auth");
    }
}


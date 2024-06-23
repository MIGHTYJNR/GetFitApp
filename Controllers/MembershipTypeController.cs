using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Models.MembershipType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetFitApp.Controllers;

[Authorize]
public class MembershipTypeController(INotyfService notyf, GetFitContext getFitDbContext) : Controller
{
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public IActionResult CreateMembershipType()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMembershipType(CreateMembershipTypeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var MembershipTypeExists = await _getFitDbContext.MembershipTypes.AnyAsync(x => x.MembershipTypeName == model.MembershipTypeName);

            if (MembershipTypeExists)
            {
                _notyfService.Warning("Membership type already exists");
                return View(model);
            }

            var membershipType = new MembershipType
            {
                MembershipTypeName = model.MembershipTypeName,
                Duration = model.Duration,
                Amount = model.Amount,
                CreatedDate = model.CreatedDate
            };

            await _getFitDbContext.MembershipTypes.AddAsync(membershipType);
            var result = await _getFitDbContext.SaveChangesAsync();

            if (result > 0)
            {
                _notyfService.Success("Created successfully");
                return RedirectToAction("ListMembershipType", "MembershipType");
            }
        }

        _notyfService.Error("An error occurred while creating membership type");
        return View(model);
    }


    [HttpGet]
    public IActionResult ListMembershipType()
    {
        var membershipTypes = _getFitDbContext.MembershipTypes
            .OrderBy(mt => mt.Duration)
            .Select(mt => new MembershipTypeViewModel
        {
            Id = mt.Id,
            MembershipTypeName = mt.MembershipTypeName,
            Duration = mt.Duration,
            Amount = mt.Amount,
        }).ToList();

        return View(membershipTypes);
    }


    [HttpGet]
    public async Task<IActionResult> UpdateMembershipType(Guid Id)
    {
        var membershipTypes = await _getFitDbContext.MembershipTypes.FindAsync(Id);

        if (membershipTypes == null)
        {
            _notyfService.Error("Membership type not found");
            return View();
        }

        var model = new MembershipTypeViewModel
        {
            Id = membershipTypes.Id,
            MembershipTypeName = membershipTypes.MembershipTypeName,
            Duration = membershipTypes.Duration,
            Amount = membershipTypes.Amount,
            
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateMembershipType(MembershipTypeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var membershipType = await _getFitDbContext.MembershipTypes.FindAsync(model.Id);

            if (membershipType == null)
            {
                _notyfService.Warning("Membership type not found");
                return View();
            }
            else
            {
                membershipType.MembershipTypeName = model.MembershipTypeName;
                membershipType.Duration = model.Duration;
                membershipType.Amount = model.Amount;
                membershipType.ModifiedDate = model.ModifiedDate;

                _getFitDbContext.Update(membershipType);
                var result = await _getFitDbContext.SaveChangesAsync();

                if (result > 0)
                {
                    _notyfService.Success("Updated successfully");
                    return RedirectToAction("ListMembershipType");
                }
                else
                {
                    _notyfService.Error("Error! update not successful");
                    return View(model);
                }
            }
        }

        _notyfService.Error("An error occurred while updating membership type");
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteMembershipType(Guid Id)
    {
        var membershipType = await _getFitDbContext.MembershipTypes.FindAsync(Id);

        if (membershipType == null)
        {
            _notyfService.Warning("Membership type not found");
            return View();
        }

        _getFitDbContext.MembershipTypes.Remove(membershipType);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Deleted successfully");
            return RedirectToAction("ListMembershipType");
        }
        else
        {
            _notyfService.Error("An error occurred while deleting membership type");
            return RedirectToAction("ListMembershipType");
        }
    }
}

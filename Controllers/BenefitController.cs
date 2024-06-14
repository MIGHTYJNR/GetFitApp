using AspNetCoreHero.ToastNotification.Abstractions;
using GetFitApp.Data;
using GetFitApp.Data.Entities;
using GetFitApp.Models.Benefit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GetFitApp.Controllers;

[Authorize]
public class BenefitController(INotyfService notyf, GetFitContext getFitDbContext) : Controller
{
    private readonly INotyfService _notyfService = notyf;
    private readonly GetFitContext _getFitDbContext = getFitDbContext;

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public IActionResult CreateBenefit()
    {
        var membershipTypes = _getFitDbContext.MembershipTypes
            .OrderBy(mt => mt.Duration)
            .Select(mt => new SelectListItem
        {
            Text = mt.MembershipTypeName,
            Value = mt.Id.ToString()
        }).ToList();

        var viewModel = new BenefitViewModel
        {
            MembershipTypes = membershipTypes
        };
        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> CreateBenefit(BenefitViewModel model)
    {
        if (ModelState.IsValid)
        {
            var benefitExists = await _getFitDbContext.Benefits.AnyAsync(x => x.Name == model.Name);

            if (benefitExists)
            {
                _notyfService.Warning("Benefit already exists");
                return View(model);
            }

            var benefit = new Benefit
            {
                Name = model.Name,
                Description = model.Description,
                MembershipTypeId = model.MembershipTypeId,
                CreatedDate = model.CreatedDate
            };

            await _getFitDbContext.Benefits.AddAsync(benefit);
            var result = await _getFitDbContext.SaveChangesAsync();

            if (result > 0)
            {
                _notyfService.Success("Created successfully");
                return RedirectToAction("ListBenefits", "Benefit");
            }
        }

        _notyfService.Error("An error occurred while creating benefit");
        return View(model);
    }


    [HttpGet]
    public IActionResult ListBenefits()
    {
        var benefit = _getFitDbContext.Benefits
            .Include(t => t.MembershipType)
            .Select(b => new BenefitViewModel
        {
            Id = b.Id,
            Name = b.Name.ToUpper(),
            Description = b.Description,
            MembershipTypeId = b.MembershipTypeId,
            MembershipTypeName = b.MembershipType.MembershipTypeName.ToUpper(),
            }).ToList();

        return View(benefit);
    }


    [HttpGet]
    public IActionResult UpdateBenefit(Guid Id)
    {
        var membershipTypes = _getFitDbContext.MembershipTypes
            .OrderBy(mt => mt.Duration)
            .Select(mt => new SelectListItem
            {
                Text = mt.MembershipTypeName,
                Value = mt.Id.ToString()
            }).ToList();

        var viewModel = new BenefitViewModel
        {
            MembershipTypes = membershipTypes
        };
        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateBenefit(BenefitViewModel model)
    {
        if (ModelState.IsValid)
        {
            var benefitExists = await _getFitDbContext.Benefits.FindAsync(model.Id);

            if (benefitExists == null)
            {
                _notyfService.Error("Benefit not found");
                return View(model);
            }
            else
            {
                benefitExists.Name = model.Name;
                benefitExists.Description = model.Description;
                benefitExists.MembershipTypeId = model.MembershipTypeId;
                benefitExists.ModifiedDate = model.ModifiedDate;

                _getFitDbContext.Update(benefitExists);
                await _getFitDbContext.SaveChangesAsync();

                _notyfService.Success("Benefit details updated successfully");
                return RedirectToAction("ListBenefits", "Benefit");
            }
        }

        _notyfService.Error("An error occurred while creating benefit");
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteBenefit(Guid Id)
    {
        var benefit = await _getFitDbContext.Benefits.FindAsync(Id);

        if (benefit == null)
        {
            _notyfService.Error("Benefit not found");
            return View();
        }

        _getFitDbContext.Benefits.Remove(benefit);
        var result = await _getFitDbContext.SaveChangesAsync();

        if (result > 0)
        {
            _notyfService.Success("Deleted successfully");
            return RedirectToAction("ListBenefits");
        }
        else
        {
            _notyfService.Error("An error occurred while deleting specialization");
            return RedirectToAction("ListBenefits");
        }
    }
}

using System.Security.Claims;
using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using App.Domain.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.ViewModels.CompanyUsers;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyAdmin")]
public class CompanyUsersController(
    ICompanyAppUserService companyAppUserService,
    ICompanyRoleService companyRoleService,
    IAppUserService appUserService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var companyId = GetCurrentCompanyId();
        if (companyId == null)
        {
            return Forbid();
        }

        var roleCodes = await GetRoleCodeMapAsync();
        var members = await companyAppUserService.GetAllByCompanyIdAsync(companyId.Value);

        var viewModel = new CompanyUserIndexViewModel
        {
            CanTransferOwnership = User.IsInRole("CompanyOwner"),
            Members = members
                .OrderByDescending(x => x.IsOwner)
                .ThenBy(x => x.AppUser!.Email)
                .Select(x => new CompanyUserMemberViewModel
                {
                    Id = x.Id,
                    Email = x.AppUser?.Email ?? string.Empty,
                    FullName = $"{x.AppUser?.FirstName} {x.AppUser?.LastName}".Trim(),
                    RoleLabel = roleCodes.TryGetValue(x.CompanyRoleId, out var code)
                        ? ToDisplayRole(code)
                        : (x.CompanyRole?.Label ?? x.CompanyRole?.Code ?? "Unknown"),
                    IsOwner = x.IsOwner,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        if (!CanManageUsers())
        {
            return Forbid();
        }

        return View(await BuildEditViewModelAsync(new CompanyAppUser
        {
            IsActive = true,
            IsOwner = false
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompanyUserEditViewModel viewModel)
    {
        if (!CanManageUsers())
        {
            return Forbid();
        }

        var companyId = GetCurrentCompanyId();
        var currentUserId = GetCurrentUserId();
        if (companyId == null || currentUserId == null)
        {
            return Forbid();
        }

        if (viewModel.CompanyAppUser == null)
        {
            return BadRequest();
        }

        var companyAppUser = viewModel.CompanyAppUser;
        companyAppUser.IsOwner = false;

        var targetAppUser = await appUserService.GetByEmailAsync(viewModel.TargetEmail.Trim());
        if (targetAppUser == null)
        {
            ModelState.AddModelError(nameof(viewModel.TargetEmail), "No account with this email exists. Ask the user to register first.");
        }
        else
        {
            companyAppUser.AppUserId = targetAppUser.Id;
        }

        if (!await IsAllowedRoleAsync(companyAppUser.CompanyRoleId))
        {
            ModelState.AddModelError(nameof(viewModel.CompanyAppUser.CompanyRoleId), "Selected role must be Admin, Manager, or Employee.");
        }

        var existingAssignments = await companyAppUserService.GetAllByCompanyIdAsync(companyId.Value);
        if (existingAssignments.Any(x => x.AppUserId == companyAppUser.AppUserId))
        {
            ModelState.AddModelError(nameof(viewModel.TargetEmail), "User is already a member of this company.");
        }

        if (ModelState.IsValid)
        {
            companyAppUser.CompanyId = companyId.Value;
            companyAppUser.CreatedByAppUserId = currentUserId.Value;
            companyAppUser.CreatedAt = DateTime.UtcNow;
            companyAppUser.UpdatedAt = null;
            companyAppUser.DeletedAt = null;
            companyAppUser.IsActive = true;

            await companyAppUserService.AddAsync(companyAppUser, companyId.Value);
            TempData["SuccessMessage"] = $"Added {targetAppUser!.Email} to the company.";
            return RedirectToAction(nameof(Index));
        }

        return View(await BuildEditViewModelAsync(companyAppUser, viewModel.TargetEmail));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferOwnership(Guid memberId)
    {
        if (!User.IsInRole("CompanyOwner"))
        {
            return Forbid();
        }

        var companyId = GetCurrentCompanyId();
        if (companyId == null)
        {
            return Forbid();
        }

        var members = await companyAppUserService.GetAllByCompanyIdAsync(companyId.Value);
        var currentOwner = members.FirstOrDefault(x => x.IsOwner && x.IsActive);
        var newOwner = members.FirstOrDefault(x => x.Id == memberId && x.IsActive);

        if (currentOwner == null || newOwner == null)
        {
            return NotFound();
        }

        var roleCodes = await GetRoleCodeMapAsync();
        var ownerRoleId = roleCodes.FirstOrDefault(x => x.Value == "owner").Key;
        var adminRoleId = roleCodes.FirstOrDefault(x => x.Value == "admin").Key;

        if (ownerRoleId == Guid.Empty || adminRoleId == Guid.Empty)
        {
            ModelState.AddModelError(string.Empty, "Company roles are not initialized correctly.");
            return RedirectToAction(nameof(Index));
        }

        if (currentOwner.Id != newOwner.Id)
        {
            currentOwner.IsOwner = false;
            if (roleCodes.TryGetValue(currentOwner.CompanyRoleId, out var existingCode) && existingCode == "owner")
            {
                currentOwner.CompanyRoleId = adminRoleId;
            }

            currentOwner.UpdatedAt = DateTime.UtcNow;

            newOwner.IsOwner = true;
            newOwner.CompanyRoleId = ownerRoleId;
            newOwner.UpdatedAt = DateTime.UtcNow;

            await companyAppUserService.UpdateAsync(currentOwner, companyId.Value);
            await companyAppUserService.UpdateAsync(newOwner, companyId.Value);
        }

        TempData["SuccessMessage"] = $"Ownership transferred to {newOwner.AppUser?.Email}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<CompanyUserEditViewModel> BuildEditViewModelAsync(CompanyAppUser companyAppUser, string? targetEmail = null)
    {
        var roles = (await companyRoleService.GetAllAsync())
            .Where(r => r.Code.Equals("admin", StringComparison.OrdinalIgnoreCase)
                        || r.Code.Equals("manager", StringComparison.OrdinalIgnoreCase)
                        || r.Code.Equals("employee", StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.Label)
            .ToList();

        return new CompanyUserEditViewModel
        {
            CompanyAppUser = companyAppUser,
            TargetEmail = targetEmail ?? string.Empty,
            CompanyRoleOptions = roles
                .Select(r => new SelectListItem(r.Label, r.Id.ToString(), r.Id == companyAppUser.CompanyRoleId))
                .ToList()
        };
    }

    private async Task<Dictionary<Guid, string>> GetRoleCodeMapAsync()
    {
        var roles = await companyRoleService.GetAllAsync();
        return roles.ToDictionary(x => x.Id, x => x.Code.ToLowerInvariant());
    }

    private static string ToDisplayRole(string code)
    {
        return code switch
        {
            "owner" => "Owner",
            "admin" => "Admin",
            "manager" => "Manager",
            "employee" => "Employee",
            _ => code
        };
    }

    private bool CanManageUsers()
    {
        return User.IsInRole("CompanyOwner") || User.IsInRole("CompanyAdmin");
    }

    private Guid? GetCurrentCompanyId()
    {
        var companyIdRaw = User.FindFirst("company_id")?.Value
                           ?? User.FindFirst("tenant_id")?.Value
                           ?? User.FindFirst("companyId")?.Value;

        return Guid.TryParse(companyIdRaw, out var companyId)
            ? companyId
            : null;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        return Guid.TryParse(userIdRaw, out var userId)
            ? userId
            : null;
    }

    private async Task<bool> IsAllowedRoleAsync(Guid roleId)
    {
        var role = await companyRoleService.GetByIdAsync(roleId);
        if (role == null)
        {
            return false;
        }

        var code = role.Code.ToLowerInvariant();
        return code is "admin" or "manager" or "employee";
    }
}

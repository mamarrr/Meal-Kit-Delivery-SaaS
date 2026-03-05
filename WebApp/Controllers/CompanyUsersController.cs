using System.Security.Claims;
using App.Contracts.BLL.Core;
using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels.CompanyUsers;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyAdmin")]
public class CompanyUsersController(
    ICompanyAppUserService companyAppUserService,
    ICompanyRoleService companyRoleService,
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    ILogger<CompanyUsersController> logger) : Controller
{
    private static readonly Dictionary<string, string> CompanyRoleCodeToIdentityRole = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = "CompanyAdmin",
        ["manager"] = "CompanyManager",
        ["employee"] = "CompanyEmployee"
    };

    private const string CompanyOwnerIdentityRole = "CompanyOwner";

    [HttpGet("/{slug}/company-users")]
    public async Task<IActionResult> Index(string slug)
    {
        var context = await BuildContextAsync(slug);
        if (context == null)
        {
            return Forbid();
        }

        return View(context);
    }

    [HttpPost("/{slug}/company-users/add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string slug, CompanyUsersManageViewModel model)
    {
        logger.LogInformation(
            "CompanyUsers/Add requested: slug={Slug}, addEmail={AddEmail}, addRoleCode={AddRoleCode}, user={User}",
            slug,
            model.AddEmail,
            model.AddRoleCode,
            User.Identity?.Name ?? "unknown");

        var context = await BuildContextAsync(slug);
        if (context == null)
        {
            logger.LogWarning("CompanyUsers/Add denied due to context mismatch: slug={Slug}, claimSlug={ClaimSlug}",
                slug,
                User.FindFirstValue("company_slug") ?? User.FindFirstValue("tenant_slug") ?? "<null>");
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            var modelErrors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => $"{x.Key}: {string.Join(" | ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                .ToList();

            logger.LogWarning(
                "CompanyUsers/Add model validation failed: slug={Slug}, errors={Errors}",
                slug,
                string.Join("; ", modelErrors));

            context.AddEmail = model.AddEmail;
            context.AddRoleCode = model.AddRoleCode;
            return View("Index", context);
        }

        try
        {
            var actorId = GetCurrentUserId();
            var created = await companyAppUserService.AddUserToCompanyByEmailAsync(context.CompanyId, actorId, model.AddEmail, model.AddRoleCode);
            if (created == null)
            {
                logger.LogInformation("CompanyUsers/Add no matching account found for email: {AddEmail}", model.AddEmail);
                ModelState.AddModelError(nameof(model.AddEmail), "No account with this email exists.");
                context.AddEmail = model.AddEmail;
                context.AddRoleCode = model.AddRoleCode;
                return View("Index", context);
            }

            await dbContext.SaveChangesAsync();
            await SyncCompanyIdentityRolesAsync(created.AppUserId);

            logger.LogInformation(
                "CompanyUsers/Add success: companyId={CompanyId}, targetAppUserId={TargetAppUserId}, roleId={RoleId}",
                context.CompanyId,
                created.AppUserId,
                created.CompanyRoleId);

            TempData["SuccessMessage"] = "User was added to company.";
            return RedirectToAction(nameof(Index), new { slug });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to add user to company for slug {Slug}", slug);
            ModelState.AddModelError(string.Empty, ex.Message);
            context.AddEmail = model.AddEmail;
            context.AddRoleCode = model.AddRoleCode;
            return View("Index", context);
        }
    }

    [HttpPost("/{slug}/company-users/role")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(string slug, Guid targetAppUserId, string roleCode)
    {
        var context = await BuildContextAsync(slug);
        if (context == null)
        {
            return Forbid();
        }

        try
        {
            var actorId = GetCurrentUserId();
            var updatedMembership = await companyAppUserService.UpdateCompanyMemberRoleAsync(context.CompanyId, actorId, targetAppUserId, roleCode);
            await dbContext.SaveChangesAsync();
            await SyncCompanyIdentityRolesAsync(updatedMembership.AppUserId);
            TempData["SuccessMessage"] = "User role updated.";
            return RedirectToAction(nameof(Index), new { slug });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to update company user role for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index), new { slug });
        }
    }

    [HttpPost("/{slug}/company-users/remove")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string slug, Guid targetAppUserId)
    {
        var context = await BuildContextAsync(slug);
        if (context == null)
        {
            return Forbid();
        }

        try
        {
            var actorId = GetCurrentUserId();
            var removedMembership = await companyAppUserService.RemoveUserFromCompanyAsync(context.CompanyId, actorId, targetAppUserId);
            await dbContext.SaveChangesAsync();
            await SyncCompanyIdentityRolesAsync(removedMembership.AppUserId);
            TempData["SuccessMessage"] = "User removed from company.";
            return RedirectToAction(nameof(Index), new { slug });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove company user for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index), new { slug });
        }
    }

    [HttpPost("/{slug}/company-users/transfer-ownership")]
    [Authorize(Policy = "CompanyOwner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferOwnership(string slug, Guid targetAppUserId)
    {
        var context = await BuildContextAsync(slug);
        if (context == null)
        {
            return Forbid();
        }

        try
        {
            var actorId = GetCurrentUserId();
            await companyAppUserService.TransferOwnershipAsync(context.CompanyId, actorId, targetAppUserId);
            await dbContext.SaveChangesAsync();

            var actorMembership = await dbContext.CompanyAppUsers
                .Include(x => x.CompanyRole)
                .FirstAsync(x => x.CompanyId == context.CompanyId && x.AppUserId == actorId && x.IsActive);
            var targetMembership = await dbContext.CompanyAppUsers
                .Include(x => x.CompanyRole)
                .FirstAsync(x => x.CompanyId == context.CompanyId && x.AppUserId == targetAppUserId && x.IsActive);

            await SyncCompanyIdentityRolesAsync(actorMembership.AppUserId);
            await SyncCompanyIdentityRolesAsync(targetMembership.AppUserId);

            TempData["SuccessMessage"] = "Company ownership transferred.";
            return RedirectToAction(nameof(Index), new { slug });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to transfer ownership for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index), new { slug });
        }
    }

    private async Task<CompanyUsersManageViewModel?> BuildContextAsync(string slug)
    {
        var companyIdRaw = User.FindFirstValue("company_id")
                           ?? User.FindFirstValue("tenant_id")
                           ?? User.FindFirstValue("companyId");
        var currentSlug = User.FindFirstValue("company_slug")
                          ?? User.FindFirstValue("tenant_slug");

        if (!Guid.TryParse(companyIdRaw, out var companyId)
            || string.IsNullOrWhiteSpace(currentSlug)
            || !string.Equals(currentSlug, slug, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var memberships = await companyAppUserService.GetActiveMembershipsByCompanyAsync(companyId);
        var assignableRoles = await companyRoleService.GetAllAssignableAsync();

        return new CompanyUsersManageViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            AssignableRoles = assignableRoles
                .Select(x => new CompanyRoleOptionViewModel
                {
                    Code = x.Code,
                    Label = x.Label
                })
                .ToList(),
            Members = memberships
                .Select(x => new CompanyMembershipViewModel
                {
                    AppUserId = x.AppUserId,
                    Email = x.AppUser?.Email ?? string.Empty,
                    FullName = $"{x.AppUser?.FirstName} {x.AppUser?.LastName}".Trim(),
                    IsOwner = x.IsOwner,
                    RoleCode = x.CompanyRole?.Code ?? string.Empty,
                    RoleLabel = x.CompanyRole?.Label ?? string.Empty
                })
                .ToList()
        };
    }

    private Guid GetCurrentUserId()
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user id.");
        }

        return userId;
    }

    private async Task SyncCompanyIdentityRolesAsync(Guid appUserId)
    {
        var user = await userManager.FindByIdAsync(appUserId.ToString());
        if (user == null)
        {
            return;
        }

        var memberships = await dbContext.CompanyAppUsers
            .Include(x => x.CompanyRole)
            .Where(x => x.AppUserId == appUserId && x.IsActive)
            .ToListAsync();

        var requiredRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (memberships.Any(x => x.IsOwner))
        {
            requiredRoles.Add(CompanyOwnerIdentityRole);
        }

        var highestRole = memberships
            .Where(x => !string.IsNullOrWhiteSpace(x.CompanyRole?.Code))
            .Select(x => x.CompanyRole!.Code)
            .Select(MapRoleCodeToIdentityRole)
            .OrderByDescending(GetRolePriority)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(highestRole))
        {
            requiredRoles.Add(highestRole!);
        }

        var existingRoles = await userManager.GetRolesAsync(user);

        foreach (var role in new[] { CompanyOwnerIdentityRole, "CompanyAdmin", "CompanyManager", "CompanyEmployee" })
        {
            var hasRole = existingRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
            var shouldHaveRole = requiredRoles.Contains(role);

            if (shouldHaveRole && !hasRole)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else if (!shouldHaveRole && hasRole)
            {
                await userManager.RemoveFromRoleAsync(user, role);
            }
        }
    }

    private static string? MapRoleCodeToIdentityRole(string? roleCode)
    {
        if (string.IsNullOrWhiteSpace(roleCode))
        {
            return null;
        }

        return CompanyRoleCodeToIdentityRole.TryGetValue(roleCode, out var identityRole)
            ? identityRole
            : null;
    }

    private static int GetRolePriority(string? roleName)
    {
        return roleName switch
        {
            "CompanyAdmin" => 3,
            "CompanyManager" => 2,
            "CompanyEmployee" => 1,
            _ => 0
        };
    }
}


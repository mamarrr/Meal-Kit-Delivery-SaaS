using System.Security.Claims;
using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using WebApp.Services;
using WebApp.ViewModels.Landing;

namespace WebApp.Controllers;

public class LandingController(TenantOnboardingService tenantOnboardingService, AppDbContext dbContext, UserManager<AppUser> userManager, ILogger<LandingController> logger) : Controller
{
    [HttpGet("/")]
    [HttpGet("/landing")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Entry));
        }

        return View();
    }

    [HttpGet("/landing/company-signup")]
    [AllowAnonymous]
    public IActionResult CompanySignup()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Entry));
        }

        return View(new TenantSignupViewModel());
    }

    [HttpGet("/landing/company-signup-auth")]
    [Authorize]
    public async Task<IActionResult> CompanySignupAuthenticated()
    {
        logger.LogInformation(
            "CompanySignupAuthenticated GET reached for user {User} with roles [{Roles}]",
            User.Identity?.Name ?? "unknown",
            string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

        if (!ActiveUserSelectionHelper.CanRegisterNewCompanyFromSidebar(User))
        {
            logger.LogInformation("CompanySignupAuthenticated GET denied by eligibility check.");
            return RedirectToAction(nameof(Entry));
        }

        var currentUser = await userManager.GetUserAsync(User);

        var model = new ExistingUserTenantSignupViewModel
        {
            CompanyContactEmail = currentUser?.Email ?? string.Empty
        };

        return View(model);
    }

    [HttpGet("/entry")]
    [HttpGet("/{slug:regex(^[[a-z0-9-]]+$)}/entry")]
    [Authorize]
    public async Task<IActionResult> Entry(string? context = null, string? slug = null)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction(nameof(Index));
        }

        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return RedirectToAction(nameof(Index));
        }

        var memberships = await dbContext.CompanyAppUsers
            .Where(x => x.AppUserId == userId && x.IsActive)
            .Select(x => new
            {
                x.CompanyId,
                CompanySlug = x.Company!.Slug,
                CompanyName = x.Company!.Name
            })
            .OrderBy(x => x.CompanyName)
            .ToListAsync();

        logger.LogInformation(
            "Entry pre-resolve: user={User}, requestedContext={RequestedContext}, requestedSlug={RequestedSlug}, membershipsCount={MembershipsCount}",
            User.Identity?.Name ?? "anonymous",
            context ?? "<null>",
            slug ?? "<null>",
            memberships.Count);

        var allowedContexts = ActiveUserSelectionHelper.GetAllowedContexts(User, memberships.Count > 0);
        var activeContext = ActiveUserSelectionHelper.ResolveValidContext(
            context,
            Request.Cookies[ActiveUserSelectionHelper.ActiveContextCookieName],
            allowedContexts);

        Response.Cookies.Append(
            ActiveUserSelectionHelper.ActiveContextCookieName,
            activeContext,
            ActiveUserSelectionHelper.CreateSelectionCookieOptions());

        var persistedCompanyCookie = Request.Cookies[ActiveUserSelectionHelper.ActiveCompanyCookieName];
        Guid? persistedCompanyId = null;
        if (Guid.TryParse(persistedCompanyCookie, out var parsedCompanyId))
        {
            persistedCompanyId = parsedCompanyId;
        }

        var slugMembership = !string.IsNullOrWhiteSpace(slug)
            ? memberships.FirstOrDefault(x => string.Equals(x.CompanySlug, slug, StringComparison.OrdinalIgnoreCase))
            : null;

        var companyMembership = slugMembership
                                ?? (persistedCompanyId != null
                                    ? memberships.FirstOrDefault(x => x.CompanyId == persistedCompanyId.Value)
                                    : null)
                                ?? memberships.FirstOrDefault();

        logger.LogInformation(
            "Entry company resolution: user={User}, persistedCompanyCookie={PersistedCompanyCookie}, matchedBySlug={MatchedBySlug}, resolvedCompanyId={ResolvedCompanyId}, resolvedCompanySlug={ResolvedCompanySlug}, activeContext={ActiveContext}",
            User.Identity?.Name ?? "anonymous",
            persistedCompanyCookie ?? "<null>",
            slugMembership != null,
            companyMembership?.CompanyId,
            companyMembership?.CompanySlug ?? "<null>",
            activeContext);

        if (companyMembership != null)
        {
            Response.Cookies.Append(
                ActiveUserSelectionHelper.ActiveCompanyCookieName,
                companyMembership.CompanyId.ToString(),
                ActiveUserSelectionHelper.CreateSelectionCookieOptions());
        }
        else
        {
            Response.Cookies.Delete(ActiveUserSelectionHelper.ActiveCompanyCookieName);
        }

        if (activeContext == ActiveUserSelectionHelper.CompanyContext && companyMembership != null)
        {
            if (string.IsNullOrWhiteSpace(slug)
                || !string.Equals(slug, companyMembership.CompanySlug, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect($"/{companyMembership.CompanySlug}/entry");
            }
        }
        else if (!string.IsNullOrWhiteSpace(slug))
        {
            return RedirectToAction(nameof(Entry), new { context = activeContext });
        }

        var defaultSlug = activeContext == ActiveUserSelectionHelper.CompanyContext
            ? companyMembership?.CompanySlug
            : User.GetDefaultOperationalSlug();
        logger.LogInformation(
            "Entry routing: user={User}, contextParam={ContextParam}, activeContext={ActiveContext}, defaultSlug={DefaultSlug}, roles=[{Roles}]",
            User.Identity?.Name ?? "anonymous",
            context ?? "<null>",
            activeContext ?? "<null>",
            defaultSlug ?? "<null>",
            string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

        ViewData["ActiveContext"] = activeContext;
        ViewData["DefaultSlug"] = defaultSlug;
        ViewData["AllowedContexts"] = allowedContexts;
        ViewData["CompanyOptions"] = memberships;
        ViewData["ActiveCompanyId"] = companyMembership?.CompanyId.ToString();
        return View();
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost("/entry/context")]
    public async Task<IActionResult> SwitchContext(string context)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return RedirectToAction(nameof(Index));
        }

        var hasActiveCompanyMembership = await dbContext.CompanyAppUsers
            .AnyAsync(x => x.AppUserId == userId && x.IsActive);

        var allowedContexts = ActiveUserSelectionHelper.GetAllowedContexts(User, hasActiveCompanyMembership);
        var requestedContext = ActiveUserSelectionHelper.NormalizeContext(context);
        var activeContext = ActiveUserSelectionHelper.ResolveValidContext(
            requestedContext,
            Request.Cookies[ActiveUserSelectionHelper.ActiveContextCookieName],
            allowedContexts);

        logger.LogInformation(
            "SwitchContext requested: user={User}, postedContext={PostedContext}, normalizedRequestedContext={RequestedContext}, resolvedContext={ResolvedContext}, hasActiveCompanyMembership={HasActiveCompanyMembership}, allowedContexts=[{AllowedContexts}], activeCompanyCookie={ActiveCompanyCookie}",
            User.Identity?.Name ?? "anonymous",
            context,
            requestedContext ?? "<null>",
            activeContext,
            hasActiveCompanyMembership,
            string.Join(",", allowedContexts),
            Request.Cookies[ActiveUserSelectionHelper.ActiveCompanyCookieName] ?? "<null>");

        Response.Cookies.Append(
            ActiveUserSelectionHelper.ActiveContextCookieName,
            activeContext,
            ActiveUserSelectionHelper.CreateSelectionCookieOptions());

        if (activeContext == ActiveUserSelectionHelper.CompanyContext)
        {
            var activeCompanyCookie = Request.Cookies[ActiveUserSelectionHelper.ActiveCompanyCookieName];
            if (Guid.TryParse(activeCompanyCookie, out var activeCompanyId))
            {
                var company = await dbContext.CompanyAppUsers
                    .Where(x => x.AppUserId == userId && x.IsActive && x.CompanyId == activeCompanyId)
                    .Select(x => new { x.Company!.Slug })
                    .FirstOrDefaultAsync();

                if (company != null && !string.IsNullOrWhiteSpace(company.Slug))
                {
                    return Redirect($"/{company.Slug}/entry");
                }
            }
        }

        return RedirectToAction(nameof(Entry), new { context = activeContext });
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost("/entry/company")]
    public async Task<IActionResult> SwitchCompany(Guid companyId)
    {
        logger.LogInformation(
            "SwitchCompany requested: user={User}, requestedCompanyId={RequestedCompanyId}, activeContextCookie={ActiveContextCookie}, activeCompanyCookie={ActiveCompanyCookie}",
            User.Identity?.Name ?? "anonymous",
            companyId,
            Request.Cookies[ActiveUserSelectionHelper.ActiveContextCookieName] ?? "<null>",
            Request.Cookies[ActiveUserSelectionHelper.ActiveCompanyCookieName] ?? "<null>");

        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return RedirectToAction(nameof(Index));
        }

        var companyMembership = await dbContext.CompanyAppUsers
            .Where(x => x.AppUserId == userId && x.IsActive && x.CompanyId == companyId)
            .Select(x => new { x.CompanyId, x.Company!.Slug })
            .FirstOrDefaultAsync();

        if (companyMembership == null)
        {
            logger.LogWarning(
                "SwitchCompany denied: user={User}, requestedCompanyId={RequestedCompanyId} is not an active membership",
                User.Identity?.Name ?? "anonymous",
                companyId);
            Response.Cookies.Delete(ActiveUserSelectionHelper.ActiveCompanyCookieName);
            return RedirectToAction(nameof(Entry));
        }

        Response.Cookies.Append(
            ActiveUserSelectionHelper.ActiveCompanyCookieName,
            companyMembership.CompanyId.ToString(),
            ActiveUserSelectionHelper.CreateSelectionCookieOptions());

        var hasActiveCompanyMembership = await dbContext.CompanyAppUsers
            .AnyAsync(x => x.AppUserId == userId && x.IsActive);
        var allowedContexts = ActiveUserSelectionHelper.GetAllowedContexts(User, hasActiveCompanyMembership);
        var activeContext = ActiveUserSelectionHelper.ResolveValidContext(
            ActiveUserSelectionHelper.CompanyContext,
            Request.Cookies[ActiveUserSelectionHelper.ActiveContextCookieName],
            allowedContexts);

        Response.Cookies.Append(
            ActiveUserSelectionHelper.ActiveContextCookieName,
            activeContext,
            ActiveUserSelectionHelper.CreateSelectionCookieOptions());

        logger.LogInformation(
            "SwitchCompany success: user={User}, resolvedCompanyId={ResolvedCompanyId}, resolvedCompanySlug={ResolvedCompanySlug}, resolvedContext={ResolvedContext}",
            User.Identity?.Name ?? "anonymous",
            companyMembership.CompanyId,
            companyMembership.Slug,
            activeContext);

        return Redirect($"/{companyMembership.Slug}/entry");
    }

    [HttpPost("/landing/signup")]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Signup(TenantSignupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(CompanySignup), model);
        }

        var (success, error) = await tenantOnboardingService.RegisterTenantWithOwnerAsync(model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Unable to complete onboarding.");
            return View(nameof(CompanySignup), model);
        }

        TempData["SuccessMessage"] = "Company created successfully. Welcome to your tenant workspace.";
        return RedirectToAction(nameof(Entry));
    }

    [HttpPost("/landing/signup-auth")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignupAuthenticated(ExistingUserTenantSignupViewModel model)
    {
        logger.LogInformation(
            "SignupAuthenticated POST reached for user {User}, company={CompanyName}",
            User.Identity?.Name ?? "unknown",
            model.CompanyName);

        if (!ActiveUserSelectionHelper.CanRegisterNewCompanyFromSidebar(User))
        {
            logger.LogInformation("SignupAuthenticated POST denied by eligibility check.");
            return RedirectToAction(nameof(Entry));
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(CompanySignupAuthenticated), model);
        }

        var currentUserId = userManager.GetUserId(User);
        if (!Guid.TryParse(currentUserId, out var appUserId))
        {
            return RedirectToAction(nameof(Index));
        }

        var (success, error) = await tenantOnboardingService.RegisterTenantForExistingUserAsync(appUserId, model);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, error ?? "Unable to complete onboarding.");
            return View(nameof(CompanySignupAuthenticated), model);
        }

        TempData["SuccessMessage"] = "Company created successfully. You have been added as CompanyOwner.";
        return RedirectToAction(nameof(Entry), new { context = ActiveUserSelectionHelper.CompanyContext });
    }
}

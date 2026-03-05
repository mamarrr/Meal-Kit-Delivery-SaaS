using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using WebApp.Services;
using WebApp.ViewModels.Landing;

namespace WebApp.Controllers;

[AllowAnonymous]
public class LandingController(TenantOnboardingService tenantOnboardingService, ILogger<LandingController> logger) : Controller
{
    [HttpGet("/")]
    [HttpGet("/landing")]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Entry));
        }

        return View();
    }

    [HttpGet("/landing/company-signup")]
    public IActionResult CompanySignup()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Entry));
        }

        return View(new TenantSignupViewModel());
    }

    [HttpGet("/entry")]
    public IActionResult Entry(string? context = null)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction(nameof(Index));
        }

        if (string.Equals(context, "customer", StringComparison.OrdinalIgnoreCase)
            || string.Equals(context, "company", StringComparison.OrdinalIgnoreCase))
        {
            Response.Cookies.Append("active_context", context!.ToLowerInvariant(), new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        var activeContext = (context ?? Request.Cookies["active_context"])?.ToLowerInvariant();

        var defaultSlug = User.GetDefaultOperationalSlug();
        logger.LogInformation(
            "Entry routing: user={User}, contextParam={ContextParam}, activeContext={ActiveContext}, defaultSlug={DefaultSlug}, roles=[{Roles}]",
            User.Identity?.Name ?? "anonymous",
            context ?? "<null>",
            activeContext ?? "<null>",
            defaultSlug ?? "<null>",
            string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

        if (User.IsInRole("SystemAdmin"))
        {
            return defaultSlug == null
                ? RedirectToAction("Index", "Companies")
                : RedirectToAction("Index", "Companies", new { slug = defaultSlug });
        }

        if (User.IsInRole("SystemBilling"))
        {
            return defaultSlug == null
                ? RedirectToAction("Index", "PlatformSubscriptionTiers")
                : RedirectToAction("Index", "PlatformSubscriptionTiers", new { slug = defaultSlug });
        }

        if (User.IsInRole("SystemSupport"))
        {
            return defaultSlug == null
                ? RedirectToAction("Index", "SupportTickets")
                : RedirectToAction("Index", "SupportTickets", new { slug = defaultSlug });
        }

        var hasCompanyAccess = User.IsInRole("CompanyOwner") || User.IsInRole("CompanyAdmin") || User.IsInRole("CompanyManager") || User.IsInRole("CompanyEmployee");

        if (User.IsInRole("Customer") && (activeContext == "customer" || !hasCompanyAccess))
        {
            logger.LogInformation("Entry decision: routing to customer subscriptions (hasCompanyAccess={HasCompanyAccess})", hasCompanyAccess);
            return defaultSlug == null
                ? RedirectToAction("Index", "MealSubscriptions")
                : RedirectToAction("Index", "MealSubscriptions", new { slug = defaultSlug });
        }

        if (hasCompanyAccess)
        {
            logger.LogInformation("Entry decision: routing to company profile");
            return defaultSlug == null
                ? RedirectToAction("Profile", "CompanySettings")
                : RedirectToAction("Profile", "CompanySettings", new { slug = defaultSlug });
        }

        if (User.IsInRole("Customer"))
        {
            logger.LogInformation("Entry decision: fallback routing to customer subscriptions");
            return RedirectToAction("Index", "MealSubscriptions");
        }

        logger.LogWarning("Entry decision: no recognized role claims for user {User}, routing to /Home/Index to avoid /entry loop", User.Identity?.Name ?? "unknown");
        return RedirectToAction("Index", "Home", new { area = "Root" });
    }

    [HttpPost("/landing/signup")]
    [ValidateAntiForgeryToken]
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
}

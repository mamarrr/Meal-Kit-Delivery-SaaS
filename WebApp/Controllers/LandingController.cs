using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using WebApp.Services;
using WebApp.ViewModels.Landing;

namespace WebApp.Controllers;

[AllowAnonymous]
public class LandingController(TenantOnboardingService tenantOnboardingService) : Controller
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
    public IActionResult Entry()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction(nameof(Index));
        }

        var defaultSlug = User.GetDefaultOperationalSlug();

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

        if (User.IsInRole("CompanyOwner") || User.IsInRole("CompanyAdmin") || User.IsInRole("CompanyManager") || User.IsInRole("CompanyEmployee"))
        {
            return defaultSlug == null
                ? RedirectToAction("Profile", "CompanySettings")
                : RedirectToAction("Profile", "CompanySettings", new { slug = defaultSlug });
        }

        if (User.IsInRole("Customer"))
        {
            return defaultSlug == null
                ? RedirectToAction("Index", "MealPlans")
                : RedirectToAction("Index", "MealPlans", new { slug = defaultSlug });
        }

        return RedirectToAction("Index", "Home");
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

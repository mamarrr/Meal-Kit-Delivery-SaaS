using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        if (User.IsInRole("SystemAdmin"))
        {
            return RedirectToAction("Index", "Companies");
        }

        if (User.IsInRole("SystemBilling"))
        {
            return RedirectToAction("Index", "PlatformSubscriptionTiers");
        }

        if (User.IsInRole("SystemSupport"))
        {
            return RedirectToAction("Index", "SupportTickets");
        }

        if (User.IsInRole("CompanyOwner") || User.IsInRole("CompanyAdmin") || User.IsInRole("CompanyManager") || User.IsInRole("CompanyEmployee"))
        {
            return RedirectToAction("Profile", "CompanySettings");
        }

        if (User.IsInRole("Customer"))
        {
            return RedirectToAction("Index", "MealPlans");
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

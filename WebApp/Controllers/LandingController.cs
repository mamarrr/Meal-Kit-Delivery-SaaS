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

        ViewData["ActiveContext"] = activeContext ?? "company";
        ViewData["DefaultSlug"] = defaultSlug;
        return View();
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

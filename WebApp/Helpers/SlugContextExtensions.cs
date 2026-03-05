using System.Security.Claims;

namespace WebApp.Helpers;

public static class SlugContextExtensions
{
    public static string? GetCompanySlug(this ClaimsPrincipal user)
    {
        return user.FindFirst("company_slug")?.Value
               ?? user.FindFirst("tenant_slug")?.Value;
    }

    public static bool HasAnyRole(this ClaimsPrincipal user, params string[] roles)
    {
        return roles.Any(user.IsInRole);
    }

    public static string? GetDefaultOperationalSlug(this ClaimsPrincipal user)
    {
        var companySlug = user.GetCompanySlug();
        if (!string.IsNullOrWhiteSpace(companySlug))
        {
            return companySlug;
        }

        return user.HasAnyRole("SystemAdmin", "SystemBilling", "SystemSupport")
            ? "system"
            : null;
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace WebApp.Helpers;

public static class ActiveUserSelectionHelper
{
    public const string ActiveContextCookieName = "active_context";
    public const string ActiveCompanyCookieName = "active_company";

    public const string CustomerContext = "customer";
    public const string CompanyContext = "company";
    public const string SystemContext = "system";

    private static readonly string[] CompanyRoles =
    [
        "CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee"
    ];

    private static readonly string[] SystemRoles =
    [
        "SystemAdmin", "SystemSupport", "SystemBilling"
    ];

    public static CookieOptions CreateSelectionCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        };
    }

    public static IReadOnlyList<string> GetAllowedContexts(
        ClaimsPrincipal user,
        bool hasActiveCompanyMembership)
    {
        var contexts = new List<string>();

        var hasCustomerAccess = user.IsInRole("Customer") || user.HasClaim("has_customer_access", "true");
        var hasCompanyAccess = hasActiveCompanyMembership || CompanyRoles.Any(user.IsInRole) || user.HasClaim("has_company_access", "true");
        var hasSystemAccess = SystemRoles.Any(user.IsInRole);

        if (hasCustomerAccess)
        {
            contexts.Add(CustomerContext);
        }

        if (hasCompanyAccess)
        {
            contexts.Add(CompanyContext);
        }

        if (hasSystemAccess)
        {
            contexts.Add(SystemContext);
        }

        if (contexts.Count == 0)
        {
            contexts.Add(CustomerContext);
        }

        return contexts;
    }

    public static string ResolveValidContext(
        string? requestedContext,
        string? persistedContext,
        IReadOnlyList<string> allowedContexts)
    {
        var normalizedRequested = NormalizeContext(requestedContext);
        if (normalizedRequested != null && allowedContexts.Contains(normalizedRequested))
        {
            return normalizedRequested;
        }

        var normalizedPersisted = NormalizeContext(persistedContext);
        if (normalizedPersisted != null && allowedContexts.Contains(normalizedPersisted))
        {
            return normalizedPersisted;
        }

        if (allowedContexts.Contains(CompanyContext))
        {
            return CompanyContext;
        }

        if (allowedContexts.Contains(CustomerContext))
        {
            return CustomerContext;
        }

        return allowedContexts.FirstOrDefault() ?? CustomerContext;
    }

    public static string? NormalizeContext(string? context)
    {
        if (string.IsNullOrWhiteSpace(context)) return null;
        var normalized = context.Trim().ToLowerInvariant();
        return normalized is CustomerContext or CompanyContext or SystemContext ? normalized : null;
    }

    public static bool CanRegisterNewCompanyFromSidebar(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        return !SystemRoles.Any(user.IsInRole);
    }
}

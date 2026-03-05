using System.Security.Claims;
using App.DAL.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;

namespace WebApp.Setup;

public class CompanyContextClaimsTransformation(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var userIdRaw = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? principal.FindFirstValue("sub")
                        ?? principal.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            return principal;
        }

        var memberships = await dbContext.CompanyAppUsers
            .Where(x => x.AppUserId == userId && x.IsActive)
            .OrderByDescending(x => x.IsOwner)
            .Select(x => new
            {
                x.CompanyId,
                CompanySlug = x.Company!.Slug
            })
            .ToListAsync();

        if (principal.Identity is ClaimsIdentity identity)
        {
            RemoveClaim(identity, "company_id");
            RemoveClaim(identity, "tenant_id");
            RemoveClaim(identity, "companyId");
            RemoveClaim(identity, "company_slug");
            RemoveClaim(identity, "tenant_slug");

            var companyRoles = await dbContext.UserRoles
                .Where(x => x.UserId == userId)
                .Join(
                    dbContext.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => role.Name)
                .Where(name => name == "CompanyOwner" || name == "CompanyAdmin" || name == "CompanyManager" || name == "CompanyEmployee")
                .ToListAsync();

            if (companyRoles.Count > 0 && !identity.HasClaim(c => c.Type == "has_company_access"))
            {
                identity.AddClaim(new Claim("has_company_access", "true"));
            }

            if (!identity.HasClaim(c => c.Type == "has_customer_access"))
            {
                var hasCustomerAccess = await dbContext.UserRoles
                    .Where(x => x.UserId == userId)
                    .Join(
                        dbContext.Roles,
                        userRole => userRole.RoleId,
                        role => role.Id,
                        (userRole, role) => role.Name)
                    .AnyAsync(name => name == "Customer");

                if (hasCustomerAccess)
                {
                    identity.AddClaim(new Claim("has_customer_access", "true"));
                }
            }

            var selectedCompanyCookie = httpContextAccessor.HttpContext?.Request.Cookies[ActiveUserSelectionHelper.ActiveCompanyCookieName];
            Guid? selectedCompanyId = null;
            if (Guid.TryParse(selectedCompanyCookie, out var parsedCompanyId))
            {
                selectedCompanyId = parsedCompanyId;
            }

            var selectedMembership = selectedCompanyId != null
                ? memberships.FirstOrDefault(m => m.CompanyId == selectedCompanyId.Value)
                : null;

            var effectiveMembership = selectedMembership ?? memberships.FirstOrDefault();

            if (effectiveMembership != null)
            {
                identity.AddClaim(new Claim("company_id", effectiveMembership.CompanyId.ToString()));
                identity.AddClaim(new Claim("tenant_id", effectiveMembership.CompanyId.ToString()));
                identity.AddClaim(new Claim("companyId", effectiveMembership.CompanyId.ToString()));

                if (!string.IsNullOrWhiteSpace(effectiveMembership.CompanySlug))
                {
                    identity.AddClaim(new Claim("company_slug", effectiveMembership.CompanySlug));
                    identity.AddClaim(new Claim("tenant_slug", effectiveMembership.CompanySlug));
                }

                if (selectedMembership == null && selectedCompanyId != null)
                {
                    httpContextAccessor.HttpContext?.Response.Cookies.Delete(ActiveUserSelectionHelper.ActiveCompanyCookieName);
                }
            }
            else
            {
                httpContextAccessor.HttpContext?.Response.Cookies.Delete(ActiveUserSelectionHelper.ActiveCompanyCookieName);
            }
        }

        return principal;
    }

    private static void RemoveClaim(ClaimsIdentity identity, string type)
    {
        var claims = identity.FindAll(type).ToList();
        foreach (var claim in claims)
        {
            identity.RemoveClaim(claim);
        }
    }
}

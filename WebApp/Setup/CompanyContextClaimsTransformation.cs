using System.Security.Claims;
using App.DAL.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Setup;

public class CompanyContextClaimsTransformation(AppDbContext dbContext) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var hasCompanyClaim = principal.HasClaim(c =>
            c.Type == "company_id" ||
            c.Type == "tenant_id" ||
            c.Type == "companyId");

        var hasCompanySlugClaim = principal.HasClaim(c =>
            c.Type == "company_slug" ||
            c.Type == "tenant_slug");

        if (hasCompanyClaim && hasCompanySlugClaim)
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

        var companyMembership = await dbContext.CompanyAppUsers
            .Where(x => x.AppUserId == userId && x.IsActive)
            .OrderByDescending(x => x.IsOwner)
            .Select(x => new
            {
                x.CompanyId,
                CompanySlug = x.Company!.Slug
            })
            .FirstOrDefaultAsync();

        if (companyMembership?.CompanyId == null)
        {
            return principal;
        }

        if (principal.Identity is ClaimsIdentity identity)
        {
            if (!hasCompanyClaim)
            {
                identity.AddClaim(new Claim("company_id", companyMembership.CompanyId.ToString()));
                identity.AddClaim(new Claim("tenant_id", companyMembership.CompanyId.ToString()));
                identity.AddClaim(new Claim("companyId", companyMembership.CompanyId.ToString()));
            }

            if (!hasCompanySlugClaim && !string.IsNullOrWhiteSpace(companyMembership.CompanySlug))
            {
                identity.AddClaim(new Claim("company_slug", companyMembership.CompanySlug));
                identity.AddClaim(new Claim("tenant_slug", companyMembership.CompanySlug));
            }
        }

        return principal;
    }
}

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

        if (hasCompanyClaim)
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

        var companyId = await dbContext.CompanyAppUsers
            .Where(x => x.AppUserId == userId && x.IsActive)
            .OrderByDescending(x => x.IsOwner)
            .Select(x => (Guid?)x.CompanyId)
            .FirstOrDefaultAsync();

        if (companyId == null)
        {
            return principal;
        }

        if (principal.Identity is ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim("company_id", companyId.Value.ToString()));
            identity.AddClaim(new Claim("tenant_id", companyId.Value.ToString()));
            identity.AddClaim(new Claim("companyId", companyId.Value.ToString()));
        }

        return principal;
    }
}

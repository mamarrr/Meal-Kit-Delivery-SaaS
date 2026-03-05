using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for CompanyRole lookup aggregate.
/// </summary>
public class CompanyRoleRepository : BaseRepository<CompanyRole, AppDbContext>, ICompanyRoleRepository
{
    private static readonly string[] AssignableRoleCodes = ["admin", "manager", "employee"];

    public CompanyRoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<CompanyRole?> GetByCodeAsync(string code)
    {
        return await RepositoryDbSet
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<ICollection<CompanyRole>> GetAllAssignableAsync()
    {
        return await RepositoryDbSet
            .Where(x => AssignableRoleCodes.Contains(x.Code))
            .OrderBy(x => x.Label)
            .ToListAsync();
    }
}


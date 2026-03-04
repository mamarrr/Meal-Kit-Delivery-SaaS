using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for CompanyRole lookup aggregate.
/// </summary>
public class CompanyRoleRepository : BaseRepository<CompanyRole, AppDbContext>, ICompanyRoleRepository
{
    public CompanyRoleRepository(AppDbContext context) : base(context)
    {
    }
}


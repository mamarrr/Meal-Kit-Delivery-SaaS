using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for Company aggregate.
/// </summary>
public class CompanyRepository : BaseRepository<Company, AppDbContext>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context)
    {
    }
}

using App.Domain.Core;
using Contracts.DAL;

namespace App.Contracts.DAL.Core;

/// <summary>
/// Repository interface for Company aggregate.
/// </summary>
public interface ICompanyRepository : IRepository<Company>
{
    // Company is the tenant root, so it doesn't need CompanyId filtering methods
}

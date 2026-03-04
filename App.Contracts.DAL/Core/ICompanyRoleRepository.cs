using App.Domain.Core;
using Contracts.DAL;

namespace App.Contracts.DAL.Core;

/// <summary>
/// Repository interface for CompanyRole lookup aggregate.
/// </summary>
public interface ICompanyRoleRepository : IRepository<CompanyRole>
{
}


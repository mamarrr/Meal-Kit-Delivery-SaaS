using App.Domain.Core;

namespace App.Contracts.BLL.Core;

public interface ICompanyRoleService : IBaseEntityService<CompanyRole>
{
    Task<CompanyRole?> GetByCodeAsync(string code);
    Task<ICollection<CompanyRole>> GetAllAssignableAsync();
}


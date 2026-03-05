using App.Domain.Core;

namespace App.Contracts.BLL.Core;

public interface ICompanyAppUserService : ITenantEntityService<CompanyAppUser>
{
    Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId, Guid companyId);
    Task<ICollection<CompanyAppUser>> GetActiveMembershipsByCompanyAsync(Guid companyId);
    Task<CompanyAppUser?> AddUserToCompanyByEmailAsync(Guid companyId, Guid actorUserId, string email, string roleCode);
    Task<CompanyAppUser> UpdateCompanyMemberRoleAsync(Guid companyId, Guid actorUserId, Guid targetAppUserId, string roleCode);
    Task<CompanyAppUser> RemoveUserFromCompanyAsync(Guid companyId, Guid actorUserId, Guid targetAppUserId);
    Task<CompanyAppUser> TransferOwnershipAsync(Guid companyId, Guid actorUserId, Guid targetAppUserId);
}


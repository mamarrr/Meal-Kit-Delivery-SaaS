using App.Contracts.BLL.Core;
using App.Contracts.BLL.Identity;
using App.Contracts.DAL.Core;
using App.Domain.Core;
using App.Domain.Identity;
using Microsoft.Extensions.Logging;

namespace App.BLL.Core;

public class CompanyAppUserService : BaseTenantService<CompanyAppUser, ICompanyAppUserRepository>, ICompanyAppUserService
{
    private readonly ICompanyRoleService _companyRoleService;
    private readonly IAppUserService _appUserService;
    private readonly ILogger<CompanyAppUserService> _logger;

    private static readonly string[] AssignableRoleCodes = ["admin", "manager", "employee"];

    public CompanyAppUserService(
        ICompanyAppUserRepository repository,
        ICompanyRoleService companyRoleService,
        IAppUserService appUserService,
        ILogger<CompanyAppUserService> logger) : base(repository)
    {
        _companyRoleService = companyRoleService;
        _appUserService = appUserService;
        _logger = logger;
    }

    protected override async Task<ICollection<CompanyAppUser>> GetAllByCompanyIdCoreAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<ICollection<CompanyAppUser>> GetAllByAppUserIdAsync(Guid appUserId, Guid companyId)
    {
        return await Repository.GetAllByAppUserIdAsync(appUserId, companyId);
    }

    public async Task<ICollection<CompanyAppUser>> GetActiveMembershipsByCompanyAsync(Guid companyId)
    {
        return await Repository.GetActiveMembershipsByCompanyAsync(companyId);
    }

    public async Task<CompanyAppUser?> AddUserToCompanyByEmailAsync(Guid companyId, Guid actorUserId, string email, string roleCode)
    {
        var actorMembership = await EnsureActiveMembershipAsync(companyId, actorUserId);
        EnsureCanManageMemberships(actorMembership);

        var targetUser = await _appUserService.GetByEmailAsync(email.Trim());
        if (targetUser == null)
        {
            return null;
        }

        var existingMembership = await Repository.GetActiveMembershipAsync(targetUser.Id, companyId);
        if (existingMembership != null)
        {
            throw new InvalidOperationException("User is already in the company.");
        }

        var role = await ResolveAssignableRoleAsync(roleCode);

        var createdMembership = new CompanyAppUser
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            AppUserId = targetUser.Id,
            CompanyRoleId = role.Id,
            IsOwner = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorUserId
        };

        return await AddAsync(createdMembership, companyId);
    }

    public async Task<CompanyAppUser> UpdateCompanyMemberRoleAsync(Guid companyId, Guid actorUserId, Guid targetAppUserId, string roleCode)
    {
        var actorMembership = await EnsureActiveMembershipAsync(companyId, actorUserId);
        EnsureCanManageMemberships(actorMembership);

        var targetMembership = await EnsureActiveMembershipAsync(companyId, targetAppUserId);
        EnsureCanMutateTarget(actorMembership, targetMembership);

        _logger.LogInformation(
            "UpdateCompanyMemberRole start: companyId={CompanyId}, actorUserId={ActorUserId}, targetAppUserId={TargetAppUserId}, requestedRoleCode={RequestedRoleCode}, currentRoleId={CurrentRoleId}, currentRoleCode={CurrentRoleCode}",
            companyId,
            actorUserId,
            targetAppUserId,
            roleCode,
            targetMembership.CompanyRoleId,
            targetMembership.CompanyRole?.Code ?? "<null>");

        var role = await ResolveAssignableRoleAsync(roleCode);
        _logger.LogInformation(
            "UpdateCompanyMemberRole resolved role: roleId={RoleId}, roleCode={RoleCode}, roleLabel={RoleLabel}",
            role.Id,
            role.Code,
            role.Label);

        targetMembership.CompanyRoleId = role.Id;
        // Keep FK/navigation consistent for EF update pipeline (detached entity attach).
        targetMembership.CompanyRole = null;
        targetMembership.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "UpdateCompanyMemberRole prepared membership update: targetAppUserId={TargetAppUserId}, newRoleId={NewRoleId}",
            targetAppUserId,
            targetMembership.CompanyRoleId);

        return await UpdateAsync(targetMembership, companyId);
    }

    public async Task<CompanyAppUser> RemoveUserFromCompanyAsync(Guid companyId, Guid actorUserId, Guid targetAppUserId)
    {
        var actorMembership = await EnsureActiveMembershipAsync(companyId, actorUserId);
        EnsureCanManageMemberships(actorMembership);

        var targetMembership = await EnsureActiveMembershipAsync(companyId, targetAppUserId);
        EnsureCanMutateTarget(actorMembership, targetMembership);

        targetMembership.IsActive = false;
        targetMembership.DeletedAt = DateTime.UtcNow;
        targetMembership.UpdatedAt = DateTime.UtcNow;

        return await UpdateAsync(targetMembership, companyId);
    }

    public async Task<CompanyAppUser> TransferOwnershipAsync(Guid companyId, Guid actorUserId, Guid targetAppUserId)
    {
        var actorMembership = await EnsureActiveMembershipAsync(companyId, actorUserId);
        if (!actorMembership.IsOwner)
        {
            throw new UnauthorizedAccessException("Only company owner can transfer ownership.");
        }

        if (actorUserId == targetAppUserId)
        {
            throw new InvalidOperationException("Ownership target must be another active company member.");
        }

        var targetMembership = await EnsureActiveMembershipAsync(companyId, targetAppUserId);
        if (targetMembership.IsOwner)
        {
            throw new InvalidOperationException("Target user is already company owner.");
        }

        var adminRole = await ResolveAssignableRoleAsync("admin");

        targetMembership.IsOwner = true;
        targetMembership.CompanyRoleId = adminRole.Id;
        targetMembership.UpdatedAt = DateTime.UtcNow;

        actorMembership.IsOwner = false;
        actorMembership.CompanyRoleId = adminRole.Id;
        actorMembership.UpdatedAt = DateTime.UtcNow;

        await UpdateAsync(targetMembership, companyId);
        return await UpdateAsync(actorMembership, companyId);
    }

    private async Task<CompanyAppUser> EnsureActiveMembershipAsync(Guid companyId, Guid appUserId)
    {
        var membership = await Repository.GetActiveMembershipAsync(appUserId, companyId);
        if (membership == null)
        {
            throw new KeyNotFoundException("Company membership was not found in active company scope.");
        }

        return membership;
    }

    private static void EnsureCanManageMemberships(CompanyAppUser actorMembership)
    {
        var roleCode = actorMembership.CompanyRole?.Code?.ToLowerInvariant();
        var isAdminOrOwner = actorMembership.IsOwner || roleCode == "admin";
        if (!isAdminOrOwner)
        {
            throw new UnauthorizedAccessException("Only company owner or company admin can manage users.");
        }
    }

    private static void EnsureCanMutateTarget(CompanyAppUser actorMembership, CompanyAppUser targetMembership)
    {
        if (targetMembership.IsOwner)
        {
            throw new InvalidOperationException("Use ownership transfer to modify owner membership.");
        }

        if (actorMembership.AppUserId == targetMembership.AppUserId && actorMembership.CompanyRole?.Code == "admin")
        {
            throw new UnauthorizedAccessException("Company admin cannot modify own company membership.");
        }
    }

    private async Task<CompanyRole> ResolveAssignableRoleAsync(string roleCode)
    {
        var normalizedCode = roleCode.Trim().ToLowerInvariant();
        if (!AssignableRoleCodes.Contains(normalizedCode))
        {
            throw new InvalidOperationException("Role is not assignable in this workflow.");
        }

        var role = await _companyRoleService.GetByCodeAsync(normalizedCode);
        if (role == null)
        {
            throw new KeyNotFoundException("Company role was not found.");
        }

        return role;
    }
}


using App.Domain.Identity;
using App.Domain.Core;
using Base.Contracts.Domain;

namespace App.Domain.Support;

public class TenantSupportAccess : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid SupportUserId { get; set; }

    public bool IsReadOnly { get; set; }
    public string Reason { get; set; } = default!;
    public DateTime GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public Guid GrantedByAppUserId { get; set; }

    public Company? Company { get; set; }
    public AppUser? SupportUser { get; set; }
    public AppUser? GrantedByAppUser { get; set; }
}

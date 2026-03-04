using App.Domain.Identity;
using App.Domain.Core;
using Base.Contracts.Domain;

namespace App.Domain.Support;

public class SupportImpersonationSession : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid SupportUserId { get; set; }
    public Guid ImpersonatedAppUserId { get; set; }

    public string Reason { get; set; } = default!;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public Company? Company { get; set; }
    public AppUser? SupportUser { get; set; }
    public AppUser? ImpersonatedAppUser { get; set; }
}

using App.Domain.Identity;
using App.Domain.Core;
using Base.Contracts.Domain;

namespace App.Domain.Support;

public class SupportTicket : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Guid? CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    public Guid? AssignedToAppUserId { get; set; }
    public Guid SupportTicketStatusId { get; set; }

    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
    public AppUser? AssignedToAppUser { get; set; }
    public SupportTicketStatus? SupportTicketStatus { get; set; }
}

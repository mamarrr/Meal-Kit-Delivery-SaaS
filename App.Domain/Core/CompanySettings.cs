using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Core;

public class CompanySettings : BaseEntity, ITenantProvider
{
    public int DefaultNoRepeatWeeks { get; set; }
    public int SelectionDeadlineDaysBeforeDelivery { get; set; }
    public bool AllowAutoSelection { get; set; }
    public bool AllowPauseSubscription { get; set; }
    public bool AllowSkipWeek { get; set; }
    public int MinimumSubscriptionWeeks { get; set; }
    public int MaxDeliveryAttempts { get; set; }
    public bool AllowRedeliveryAfterFailure { get; set; }
    public int ComplaintEscalationThreshold { get; set; }
    public int ComplaintEscalationDaysWindow { get; set; }
    public bool AutoPrioritizeFreshestStock { get; set; }
    public bool AutoAssignEarliestSlot { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid? UpdatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public AppUser? UpdatedByAppUser { get; set; }
}

using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class PricingAdjustment : BaseEntity, ITenantProvider
{
    public string AdjustmentType { get; set; } = default!; // delivery_fee | discount
    public string Label { get; set; } = default!;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }

    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}


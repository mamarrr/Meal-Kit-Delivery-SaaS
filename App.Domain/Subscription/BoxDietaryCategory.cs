using App.Domain.Identity;
using App.Domain.Menu;
using Base.Contracts.Domain;

namespace App.Domain.Subscription;

public class BoxDietaryCategory : BaseEntity, ITenantProvider
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Guid BoxId { get; set; }
    public Guid DietaryCategoryId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CreatedByAppUserId { get; set; }

    public Box? Box { get; set; }
    public DietaryCategory? DietaryCategory { get; set; }
    public Company? Company { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}

using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Core;

public class CompanyAppUser : BaseEntity
{
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid AppUserId { get; set; }
    public Guid CompanyRoleId { get; set; }
    public Guid CreatedByAppUserId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public AppUser? AppUser { get; set; }
    public CompanyRole? CompanyRole { get; set; }
    public AppUser? CreatedByAppUser { get; set; }
}

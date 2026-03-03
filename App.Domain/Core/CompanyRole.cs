using Base.Contracts.Domain;

namespace App.Domain.Core;

public class CompanyRole : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
    
    // Navigation Properties
    public ICollection<CompanyAppUser>? CompanyAppUsers { get; set; }
}

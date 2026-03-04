using App.Domain.Identity;
using Base.Contracts.Domain;

namespace App.Domain.Support;

public class SystemSetting : BaseEntity
{
    public string Category { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string ValueType { get; set; } = default!;
    public bool IsSensitive { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? UpdatedByAppUserId { get; set; }

    public AppUser? UpdatedByAppUser { get; set; }
}

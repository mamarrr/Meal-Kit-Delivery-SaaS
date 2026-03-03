using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class QualityComplaintType : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
    
    // Navigation Properties
    public ICollection<QualityComplaint>? QualityComplaints { get; set; }
}

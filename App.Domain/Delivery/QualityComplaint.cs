using Base.Contracts.Domain;

namespace App.Domain.Delivery;

public class QualityComplaint : BaseEntity, ITenantProvider
{
    public int Severity { get; set; }
    public string Description { get; set; } = default!;
    public DateTime? EscalatedAt { get; set; }
    public string? EscalationAction { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Foreign Keys
    public Guid CompanyId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DeliveryId { get; set; }
    public Guid QualityComplaintTypeId { get; set; }
    public Guid QualityComplaintStatusId { get; set; }
    
    // Navigation Properties
    public Company? Company { get; set; }
    public Customer? Customer { get; set; }
    public Delivery? Delivery { get; set; }
    public QualityComplaintType? QualityComplaintType { get; set; }
    public QualityComplaintStatus? QualityComplaintStatus { get; set; }
}

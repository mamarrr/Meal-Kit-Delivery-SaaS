namespace WebApp.ViewModels.Delivery;

public class CompanyComplaintsIndexViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;
    public List<CompanyComplaintListItemViewModel> Complaints { get; set; } = [];
}

public class CompanyComplaintListItemViewModel
{
    public Guid ComplaintId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string DeliveryReference { get; set; } = string.Empty;
    public string TypeLabel { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public int Severity { get; set; }
}

public class CompanyComplaintDetailsViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;
    public Guid ComplaintId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? EscalatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string DeliveryReference { get; set; } = string.Empty;
    public string TypeLabel { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public int Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EscalationAction { get; set; }
}

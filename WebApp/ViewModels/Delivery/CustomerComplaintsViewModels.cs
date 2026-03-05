using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.Delivery;

public class CustomerComplaintsIndexViewModel
{
    public List<CustomerComplaintListItemViewModel> Complaints { get; set; } = [];
}

public class CustomerComplaintListItemViewModel
{
    public Guid ComplaintId { get; set; }
    public Guid DeliveryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string TypeLabel { get; set; } = string.Empty;
    public int Severity { get; set; }
}

public class CustomerComplaintDetailsViewModel
{
    public Guid ComplaintId { get; set; }
    public Guid DeliveryId { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public int Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? EscalatedAt { get; set; }
    public string? EscalationAction { get; set; }
}

public class CustomerComplaintCreateViewModel
{
    [Required]
    [Display(Name = "Delivery")]
    public Guid? DeliveryId { get; set; }

    [Required]
    [Display(Name = "Type")]
    public Guid? ComplaintTypeId { get; set; }

    [Range(1, 5)]
    [Display(Name = "Severity (1-5)")]
    public int Severity { get; set; } = 3;

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    public List<SelectListItem> DeliveryOptions { get; set; } = [];
    public List<SelectListItem> ComplaintTypeOptions { get; set; } = [];
}

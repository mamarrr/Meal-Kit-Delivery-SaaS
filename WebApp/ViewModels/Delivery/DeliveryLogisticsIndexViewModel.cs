using App.Domain.Delivery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.Delivery;

public class DeliveryLogisticsIndexViewModel
{
    public Guid CompanyId { get; set; }
    public string CompanySlug { get; set; } = string.Empty;

    public DateTime DeliveryDate { get; set; }

    public List<DeliveryZone> Zones { get; set; } = [];
    public List<DeliveryWindow> ZoneSchedules { get; set; } = [];
    public List<DeliveryRunOrderListItem> RunsAndOrders { get; set; } = [];

    public DeliveryZoneFormViewModel ZoneForm { get; set; } = new();
    public DeliveryWindowFormViewModel WindowForm { get; set; } = new();
    public DeliveryCreateFormViewModel DeliveryCreateForm { get; set; } = new();
    public DeliveryTrackingFormViewModel TrackingForm { get; set; } = new();
    public DeliveryEscalationRulesFormViewModel EscalationForm { get; set; } = new();

    public List<SelectListItem> CustomerOptions { get; set; } = [];
    public List<SelectListItem> ZoneOptions { get; set; } = [];
    public List<SelectListItem> WindowOptions { get; set; } = [];
    public List<SelectListItem> DeliveryStatusOptions { get; set; } = [];
    public List<SelectListItem> AttemptResultOptions { get; set; } = [];
}

public class DeliveryZoneFormViewModel
{
    public Guid? DeliveryZoneId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DeliveryRunOrderListItem
{
    public Guid DeliveryId { get; set; }
    public string RunReference { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string AddressSummary { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    public string TimeWindow { get; set; } = string.Empty;
    public string StatusLabel { get; set; } = string.Empty;
    public DateTime ScheduledTime { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? FailureReason { get; set; }
}

public class DeliveryWindowFormViewModel
{
    public Guid? DeliveryWindowId { get; set; }
    public Guid DeliveryZoneId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int? Capacity { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DeliveryCreateFormViewModel
{
    public Guid CustomerId { get; set; }
    public Guid DeliveryZoneId { get; set; }
    public Guid DeliveryWindowId { get; set; }
    public DateTime ScheduledDate { get; set; }
}

public class DeliveryTrackingFormViewModel
{
    public Guid DeliveryId { get; set; }
    public Guid DeliveryStatusId { get; set; }
    public Guid DeliveryAttemptResultId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? ProofReference { get; set; }
    public DateTime? RescheduleTo { get; set; }
}

public class DeliveryEscalationRulesFormViewModel
{
    public int ComplaintEscalationThreshold { get; set; } = 2;
    public int ComplaintEscalationDaysWindow { get; set; } = 14;
    public bool AutoPrioritizeFreshestStock { get; set; } = true;
    public bool AutoAssignEarliestSlot { get; set; } = true;
}


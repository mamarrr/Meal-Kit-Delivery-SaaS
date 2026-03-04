using App.Domain.Delivery;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.QualityComplaints;

public class QualityComplaintEditViewModel
{
    public QualityComplaint QualityComplaint { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CustomerOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> DeliveryOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> QualityComplaintTypeOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> QualityComplaintStatusOptions { get; set; } = [];
}

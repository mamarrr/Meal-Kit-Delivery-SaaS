using App.Domain.Support;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.SupportTickets;

public class SupportTicketEditViewModel
{
    public SupportTicket SupportTicket { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CompanyOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> CreatedByUserOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> AssignedToUserOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> StatusOptions { get; set; } = [];
}

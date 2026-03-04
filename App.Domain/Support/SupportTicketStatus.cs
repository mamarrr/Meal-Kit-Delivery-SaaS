using Base.Contracts.Domain;

namespace App.Domain.Support;

public class SupportTicketStatus : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;

    public ICollection<SupportTicket>? SupportTickets { get; set; }
}

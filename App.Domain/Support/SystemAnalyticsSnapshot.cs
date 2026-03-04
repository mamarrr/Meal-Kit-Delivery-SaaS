using Base.Contracts.Domain;

namespace App.Domain.Support;

public class SystemAnalyticsSnapshot : BaseEntity
{
    public DateTime CapturedAt { get; set; }
    public int ActiveCompanies { get; set; }
    public int ActiveSubscribers { get; set; }
    public int WeeklyDeliveries { get; set; }
    public int OpenSupportTickets { get; set; }
}

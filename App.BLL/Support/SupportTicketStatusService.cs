using App.Contracts.BLL.Support;
using App.Contracts.DAL.Support;
using App.Domain.Support;

namespace App.BLL.Support;

public class SupportTicketStatusService : BaseService<SupportTicketStatus, ISupportTicketStatusRepository>, ISupportTicketStatusService
{
    public SupportTicketStatusService(ISupportTicketStatusRepository repository) : base(repository)
    {
    }
}

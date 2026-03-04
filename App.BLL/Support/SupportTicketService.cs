using App.Contracts.BLL.Support;
using App.Contracts.DAL.Support;
using App.Domain.Support;

namespace App.BLL.Support;

public class SupportTicketService : BaseService<SupportTicket, ISupportTicketRepository>, ISupportTicketService
{
    public SupportTicketService(ISupportTicketRepository repository) : base(repository)
    {
    }
}

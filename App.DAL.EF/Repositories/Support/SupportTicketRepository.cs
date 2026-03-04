using App.Contracts.DAL.Support;
using App.Domain.Support;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Support;

public class SupportTicketRepository : BaseRepository<SupportTicket, AppDbContext>, ISupportTicketRepository
{
    public SupportTicketRepository(AppDbContext context) : base(context)
    {
    }
}

using App.Contracts.DAL.Support;
using App.Domain.Support;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Support;

public class SupportTicketStatusRepository : BaseRepository<SupportTicketStatus, AppDbContext>, ISupportTicketStatusRepository
{
    public SupportTicketStatusRepository(AppDbContext context) : base(context)
    {
    }
}

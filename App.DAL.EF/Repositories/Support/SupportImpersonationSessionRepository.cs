using App.Contracts.DAL.Support;
using App.Domain.Support;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Support;

public class SupportImpersonationSessionRepository : BaseRepository<SupportImpersonationSession, AppDbContext>, ISupportImpersonationSessionRepository
{
    public SupportImpersonationSessionRepository(AppDbContext context) : base(context)
    {
    }
}

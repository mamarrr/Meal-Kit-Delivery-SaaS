using App.Contracts.DAL.Support;
using App.Domain.Support;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Support;

public class SystemAnalyticsSnapshotRepository : BaseRepository<SystemAnalyticsSnapshot, AppDbContext>, ISystemAnalyticsSnapshotRepository
{
    public SystemAnalyticsSnapshotRepository(AppDbContext context) : base(context)
    {
    }
}

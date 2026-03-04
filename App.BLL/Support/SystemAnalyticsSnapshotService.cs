using App.Contracts.BLL.Support;
using App.Contracts.DAL.Support;
using App.Domain.Support;

namespace App.BLL.Support;

public class SystemAnalyticsSnapshotService : BaseService<SystemAnalyticsSnapshot, ISystemAnalyticsSnapshotRepository>, ISystemAnalyticsSnapshotService
{
    public SystemAnalyticsSnapshotService(ISystemAnalyticsSnapshotRepository repository) : base(repository)
    {
    }
}

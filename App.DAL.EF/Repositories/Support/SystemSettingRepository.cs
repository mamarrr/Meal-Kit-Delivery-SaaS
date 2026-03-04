using App.Contracts.DAL.Support;
using App.Domain.Support;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories.Support;

public class SystemSettingRepository : BaseRepository<SystemSetting, AppDbContext>, ISystemSettingRepository
{
    public SystemSettingRepository(AppDbContext context) : base(context)
    {
    }
}

using App.Contracts.BLL.Support;
using App.Contracts.DAL.Support;
using App.Domain.Support;

namespace App.BLL.Support;

public class SystemSettingService : BaseService<SystemSetting, ISystemSettingRepository>, ISystemSettingService
{
    public SystemSettingService(ISystemSettingRepository repository) : base(repository)
    {
    }
}

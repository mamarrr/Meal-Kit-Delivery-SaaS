using App.Domain.Support;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.SystemSettings;

public class SystemSettingEditViewModel
{
    public SystemSetting SystemSetting { get; set; } = default!;

    public IReadOnlyList<SelectListItem> UpdatedByUserOptions { get; set; } = [];
}

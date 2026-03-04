using App.Domain.Support;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.SupportAccess;

public class SupportAccessEditViewModel
{
    public TenantSupportAccess TenantSupportAccess { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CompanyOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> GrantedByUserOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> SupportUserOptions { get; set; } = [];
}

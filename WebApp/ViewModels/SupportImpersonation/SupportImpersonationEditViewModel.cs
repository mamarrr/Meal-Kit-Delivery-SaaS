using App.Domain.Support;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.SupportImpersonation;

public class SupportImpersonationEditViewModel
{
    public SupportImpersonationSession SupportImpersonationSession { get; set; } = default!;

    public IReadOnlyList<SelectListItem> CompanyOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> SupportUserOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> ImpersonatedUserOptions { get; set; } = [];
}

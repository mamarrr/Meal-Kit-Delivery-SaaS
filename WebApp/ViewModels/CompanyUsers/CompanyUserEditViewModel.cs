using App.Domain.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.CompanyUsers;

public class CompanyUserEditViewModel
{
    public CompanyAppUser CompanyAppUser { get; set; } = default!;

    public string TargetEmail { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> CompanyRoleOptions { get; set; } = [];
}


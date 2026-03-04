using App.Domain.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels.CompanyUsers;

public class CompanyUserEditViewModel
{
    public CompanyAppUser CompanyAppUser { get; set; } = default!;

    public IReadOnlyList<SelectListItem> AppUserOptions { get; set; } = [];

    public IReadOnlyList<SelectListItem> CompanyRoleOptions { get; set; } = [];
}


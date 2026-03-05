using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.CompanyUsers;

public class CompanyUsersManageViewModel
{
    public Guid CompanyId { get; set; }
    public string? CompanySlug { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "User email")]
    public string AddEmail { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public string AddRoleCode { get; set; } = "employee";

    public List<CompanyRoleOptionViewModel> AssignableRoles { get; set; } = [];

    public List<CompanyMembershipViewModel> Members { get; set; } = [];
}

public class CompanyRoleOptionViewModel
{
    public string Code { get; set; } = default!;
    public string Label { get; set; } = default!;
}

public class CompanyMembershipViewModel
{
    public Guid AppUserId { get; set; }
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public bool IsOwner { get; set; }
    public string RoleCode { get; set; } = default!;
    public string RoleLabel { get; set; } = default!;
}


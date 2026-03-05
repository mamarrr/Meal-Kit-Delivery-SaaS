namespace WebApp.ViewModels.CompanyUsers;

public class CompanyUserMemberViewModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string RoleLabel { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

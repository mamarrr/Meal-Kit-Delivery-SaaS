namespace WebApp.ViewModels.CompanyUsers;

public class CompanyUserIndexViewModel
{
    public bool CanTransferOwnership { get; set; }
    public IReadOnlyList<CompanyUserMemberViewModel> Members { get; set; } = [];
}

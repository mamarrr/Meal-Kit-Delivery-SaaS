using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Landing;

public class ExistingUserTenantSignupViewModel
{
    [Required]
    [MaxLength(128)]
    [Display(Name = "Company name")]
    public string CompanyName { get; set; } = default!;

    [Required]
    [MaxLength(32)]
    [Display(Name = "Registration number")]
    public string RegistrationNumber { get; set; } = default!;

    [Required]
    [EmailAddress]
    [Display(Name = "Company contact email")]
    public string CompanyContactEmail { get; set; } = default!;

    [Required]
    [MaxLength(64)]
    [Display(Name = "Company contact phone")]
    public string CompanyContactPhone { get; set; } = default!;

    [Required]
    [Url]
    [Display(Name = "Company website")]
    public string CompanyWebsiteUrl { get; set; } = default!;
}

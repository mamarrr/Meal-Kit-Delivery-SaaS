using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Landing;

public class TenantSignupViewModel
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

    [Required]
    [MaxLength(128)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = default!;

    [Required]
    [MaxLength(128)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = default!;

    [Required]
    [EmailAddress]
    [Display(Name = "Owner email")]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = default!;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = default!;
}

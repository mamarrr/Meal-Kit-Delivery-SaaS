using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1.Identity;

public class Register
{
    [MaxLength(128)]
    [Required]
    public string FirstName { get; set; } = default!;

    [MaxLength(128)]
    [Required]
    public string LastName { get; set; } = default!;

    [MaxLength(256)]
    [EmailAddress]
    [Required]
    public string Email { get; set; } = default!;
        
    [MinLength(6)]
    [MaxLength(100)]
    [Required]
    public string Password { get; set; } = default!;
}

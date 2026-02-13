using System.ComponentModel.DataAnnotations;

namespace MoneyMood.Dtos.Auth;

public class SignUpRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Le mot de passe doit contenir au moins : une minuscule, une majuscule et un chiffre")]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
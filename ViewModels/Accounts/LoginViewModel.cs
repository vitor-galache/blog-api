using System.ComponentModel.DataAnnotations;

namespace BlogApi.ViewModels.Accounts;

public class LoginViewModel
{
    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Informe a senha")]
    public string Password { get; set; } = null!;
}
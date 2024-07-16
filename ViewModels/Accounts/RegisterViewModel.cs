using System.ComponentModel.DataAnnotations;

namespace BlogApi.ViewModels.Accounts;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Insira um nome")]
    [MinLength(2,ErrorMessage = "Nome precisa conter 2 caracteres no minímo") ]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "Insira o email")]
    [EmailAddress(ErrorMessage = "Email inserido inválido")]
    public string Email { get; set; } = null!;
}
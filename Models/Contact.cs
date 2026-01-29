using System.ComponentModel.DataAnnotations;

namespace ContatosApp.Models;

public class Contact
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório.")]
    [MinLength(6, ErrorMessage = "Nome deve ter mais de 5 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Telefone deve ter exatamente 9 dígitos.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = string.Empty;

    //Soft delete
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAtUtc { get; set; }
}

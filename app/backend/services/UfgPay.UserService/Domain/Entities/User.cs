namespace UfgPay.UserService.Domain.Entities;

/// <summary>
/// Entidade de domínio representando o Usuário Acadêmico no ecossistema UFG Pay.
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Matricula { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string TipoVinculo { get; set; } = string.Empty; // discente, docente, tecnico, terceirizado
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Construtor padrão para o Entity Framework Core.
    /// </summary>
    public User() { }

    /// <summary>
    /// Fábrica para criação de nova instância de usuário com validações iniciais.
    /// </summary>
    public static User Create(string matricula, string nome, string email, string senhaHash, string tipoVinculo)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Matricula = matricula.Trim(),
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            SenhaHash = senhaHash,
            TipoVinculo = tipoVinculo.Trim().ToLowerInvariant(),
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };
    }
}

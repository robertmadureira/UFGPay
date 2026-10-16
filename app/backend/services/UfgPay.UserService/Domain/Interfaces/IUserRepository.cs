using UfgPay.UserService.Domain.Entities;

namespace UfgPay.UserService.Domain.Interfaces;

/// <summary>
/// Contrato de repositório para persistência da entidade User (DIP - Dependency Inversion Principle).
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Busca um usuário pelo identificador único global (GUID).
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um usuário pelo número de matrícula acadêmica.
    /// </summary>
    Task<User?> GetByMatriculaAsync(string matricula, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um usuário pelo endereço de e-mail institucional.
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona um novo usuário no repositório de dados.
    /// </summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Salva as alterações pendentes no contexto do banco de dados.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

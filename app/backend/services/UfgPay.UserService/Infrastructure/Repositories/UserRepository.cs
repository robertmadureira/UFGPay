using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UfgPay.UserService.Domain.Entities;
using UfgPay.UserService.Domain.Interfaces;
using UfgPay.UserService.Infrastructure.Data;

namespace UfgPay.UserService.Infrastructure.Repositories;

/// <summary>
/// Implementação EF Core de persistência para IUserRepository.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(UserDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Busca um usuário pelo GUID.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[UserRepository.GetByIdAsync] Querying user by Id: {UserId}", id);
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Busca um usuário pela matrícula.
    /// </summary>
    public async Task<User?> GetByMatriculaAsync(string matricula, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[UserRepository.GetByMatriculaAsync] Querying user by Matricula: {Matricula}", matricula);
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Matricula == matricula, cancellationToken);
    }

    /// <summary>
    /// Busca um usuário pelo e-mail.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[UserRepository.GetByEmailAsync] Querying user by Email: {Email}", email);
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Adiciona novo usuário à tabela.
    /// </summary>
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[UserRepository.AddAsync] Adding user to DbSet: {UserId}", user.Id);
        await _context.Usuarios.AddAsync(user, cancellationToken);
    }

    /// <summary>
    /// Salva as alterações pendentes.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[UserRepository.SaveChangesAsync] Saving changes to PostgreSQL database");
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

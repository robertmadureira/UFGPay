using Microsoft.Extensions.Logging;
using UfgPay.UserService.Application.Interfaces;

namespace UfgPay.UserService.Infrastructure.Security;

/// <summary>
/// Implementação de hashing com BCrypt para senhas.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private readonly ILogger<PasswordHasher> _logger;

    public PasswordHasher(ILogger<PasswordHasher> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gera o hash seguro da senha utilizando BCrypt.
    /// </summary>
    public string HashPassword(string plainPassword)
    {
        _logger.LogDebug("[PasswordHasher.HashPassword] Gerando hash BCrypt");
        return BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 11);
    }

    /// <summary>
    /// Valida a senha contra o hash armazenado.
    /// </summary>
    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        _logger.LogDebug("[PasswordHasher.VerifyPassword] Verificando correspondência da senha");
        try
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[PasswordHasher.VerifyPassword] Exceção ao verificar hash da senha");
            return false;
        }
    }
}

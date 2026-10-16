namespace UfgPay.UserService.Application.Interfaces;

/// <summary>
/// Contrato para geração e verificação de hashes de senhas seguras (SRP).
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera o hash seguro com salt para a senha em texto plano informada.
    /// </summary>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Verifica se a senha em texto plano confere com o hash armazenado.
    /// </summary>
    bool VerifyPassword(string plainPassword, string passwordHash);
}

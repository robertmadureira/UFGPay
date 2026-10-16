using UfgPay.UserService.Application.DTOs;

namespace UfgPay.UserService.Application.Interfaces;

/// <summary>
/// Contrato do serviço de aplicação para gerenciamento do ciclo de vida de usuários.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Realiza a autenticação de credenciais acadêmicas por matrícula e senha.
    /// </summary>
    Task<AuthenticationResultDto> AuthenticateAsync(string matricula, string senha, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta os dados cadastrais de um usuário pelo identificador GUID.
    /// </summary>
    Task<UserResultDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra um novo usuário acadêmico no sistema.
    /// </summary>
    Task<UserResultDto> RegisterAsync(string matricula, string nome, string email, string senha, string tipoVinculo, CancellationToken cancellationToken = default);
}

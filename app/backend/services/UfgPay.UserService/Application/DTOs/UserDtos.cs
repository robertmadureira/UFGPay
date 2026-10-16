namespace UfgPay.UserService.Application.DTOs;

/// <summary>
/// DTO de resposta com os dados consolidados do usuário.
/// </summary>
public record UserResultDto(
    Guid Id,
    string Matricula,
    string Nome,
    string Email,
    string TipoVinculo,
    bool Ativo
);

/// <summary>
/// DTO com dados de autenticação bem-sucedida ou falha.
/// </summary>
public record AuthenticationResultDto(
    bool Sucesso,
    Guid? UserId = null,
    string? Nome = null,
    string? Email = null,
    string? TipoVinculo = null,
    string? ErroMensagem = null
);

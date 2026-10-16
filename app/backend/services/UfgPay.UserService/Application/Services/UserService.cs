using Microsoft.Extensions.Logging;
using UfgPay.UserService.Application.DTOs;
using UfgPay.UserService.Application.Interfaces;
using UfgPay.UserService.Domain.Entities;
using UfgPay.UserService.Domain.Interfaces;

namespace UfgPay.UserService.Application.Services;

/// <summary>
/// Implementação das regras de negócio e casos de uso de Usuário.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Realiza a autenticação de credenciais acadêmicas por matrícula e senha.
    /// </summary>
    public async Task<AuthenticationResultDto> AuthenticateAsync(string matricula, string senha, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[UserService.AuthenticateAsync] Iniciando autenticação para Matrícula: {Matricula}", matricula);

        if (string.IsNullOrWhiteSpace(matricula) || string.IsNullOrWhiteSpace(senha))
        {
            _logger.LogWarning("[UserService.AuthenticateAsync] Falha na validação prévia: Matrícula ou senha vazia");
            return new AuthenticationResultDto(false, ErroMensagem: "Matrícula e senha são obrigatórias.");
        }

        var user = await _userRepository.GetByMatriculaAsync(matricula.Trim(), cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("[UserService.AuthenticateAsync] Usuário não encontrado para Matrícula: {Matricula}", matricula);
            return new AuthenticationResultDto(false, ErroMensagem: "Credenciais inválidas.");
        }

        if (!user.Ativo)
        {
            _logger.LogWarning("[UserService.AuthenticateAsync] Usuário inativo para UserId: {UserId}", user.Id);
            return new AuthenticationResultDto(false, ErroMensagem: "Conta inativa.");
        }

        bool senhaValida = _passwordHasher.VerifyPassword(senha, user.SenhaHash);
        if (!senhaValida)
        {
            _logger.LogWarning("[UserService.AuthenticateAsync] Senha inválida para Matrícula: {Matricula}", matricula);
            return new AuthenticationResultDto(false, ErroMensagem: "Credenciais inválidas.");
        }

        _logger.LogInformation("[UserService.AuthenticateAsync] Autenticação bem-sucedida para UserId: {UserId}, Nome: {Nome}", user.Id, user.Nome);

        return new AuthenticationResultDto(
            Sucesso: true,
            UserId: user.Id,
            Nome: user.Nome,
            Email: user.Email,
            TipoVinculo: user.TipoVinculo
        );
    }

    /// <summary>
    /// Consulta os dados cadastrais de um usuário pelo identificador GUID.
    /// </summary>
    public async Task<UserResultDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[UserService.GetByIdAsync] Consultando usuário por ID: {UserId}", id);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("[UserService.GetByIdAsync] Usuário não encontrado para ID: {UserId}", id);
            return null;
        }

        return new UserResultDto(
            user.Id,
            user.Matricula,
            user.Nome,
            user.Email,
            user.TipoVinculo,
            user.Ativo
        );
    }

    /// <summary>
    /// Registra um novo usuário acadêmico no sistema.
    /// </summary>
    public async Task<UserResultDto> RegisterAsync(string matricula, string nome, string email, string senha, string tipoVinculo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[UserService.RegisterAsync] Iniciando cadastro para Matrícula: {Matricula}, Email: {Email}", matricula, email);

        var existingUser = await _userRepository.GetByMatriculaAsync(matricula.Trim(), cancellationToken);
        if (existingUser != null)
        {
            _logger.LogError("[UserService.RegisterAsync] Matrícula já cadastrada: {Matricula}", matricula);
            throw new InvalidOperationException($"Já existe um usuário cadastrado com a matrícula {matricula}.");
        }

        var existingEmail = await _userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant(), cancellationToken);
        if (existingEmail != null)
        {
            _logger.LogError("[UserService.RegisterAsync] E-mail já cadastrado: {Email}", email);
            throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail {email}.");
        }

        string senhaHash = _passwordHasher.HashPassword(senha);
        var novoUsuario = User.Create(matricula, nome, email, senhaHash, tipoVinculo);

        await _userRepository.AddAsync(novoUsuario, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[UserService.RegisterAsync] Usuário cadastrado com sucesso. Novo UserId: {UserId}", novoUsuario.Id);

        return new UserResultDto(
            novoUsuario.Id,
            novoUsuario.Matricula,
            novoUsuario.Nome,
            novoUsuario.Email,
            novoUsuario.TipoVinculo,
            novoUsuario.Ativo
        );
    }
}

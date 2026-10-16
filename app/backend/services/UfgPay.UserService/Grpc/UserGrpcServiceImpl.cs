using Grpc.Core;
using Microsoft.Extensions.Logging;
using UfgPay.UserService.Application.Interfaces;
using UfgPay.UserService.Grpc;

namespace UfgPay.UserService.Grpc;

/// <summary>
/// Adaptador de apresentação gRPC para o serviço de Usuários e Autenticação.
/// </summary>
public class UserGrpcServiceImpl : UserGrpcService.UserGrpcServiceBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcServiceImpl> _logger;

    public UserGrpcServiceImpl(IUserService userService, ILogger<UserGrpcServiceImpl> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// RPC de autenticação acadêmica com matrícula e senha.
    /// </summary>
    public override async Task<AuthResponse> Authenticate(AuthRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[UserGrpcServiceImpl.Authenticate] Recebida solicitação gRPC para matrícula: {Matricula}", request.Matricula);

        try
        {
            var result = await _userService.AuthenticateAsync(request.Matricula, request.Senha, context.CancellationToken);

            return new AuthResponse
            {
                Sucesso = result.Sucesso,
                UserId = result.UserId?.ToString() ?? string.Empty,
                Nome = result.Nome ?? string.Empty,
                Email = result.Email ?? string.Empty,
                TipoVinculo = result.TipoVinculo ?? string.Empty,
                ErroMensagem = result.ErroMensagem ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserGrpcServiceImpl.Authenticate] Falha inesperada durante autenticação");
            return new AuthResponse
            {
                Sucesso = false,
                ErroMensagem = "Erro interno ao processar autenticação."
            };
        }
    }

    /// <summary>
    /// RPC para obtenção de dados de usuário por identificador GUID.
    /// </summary>
    public override async Task<UserResponse> GetById(UserByIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[UserGrpcServiceImpl.GetById] Recebida solicitação gRPC para UserId: {UserId}", request.UserId);

        if (!Guid.TryParse(request.UserId, out var guid))
        {
            _logger.LogWarning("[UserGrpcServiceImpl.GetById] GUID fornecido é inválido: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ID de usuário em formato inválido."));
        }

        var user = await _userService.GetByIdAsync(guid, context.CancellationToken);
        if (user == null)
        {
            _logger.LogWarning("[UserGrpcServiceImpl.GetById] Usuário não encontrado no banco: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.NotFound, "Usuário não encontrado."));
        }

        return new UserResponse
        {
            UserId = user.Id.ToString(),
            Matricula = user.Matricula,
            Nome = user.Nome,
            Email = user.Email,
            TipoVinculo = user.TipoVinculo,
            Ativo = user.Ativo
        };
    }

    /// <summary>
    /// RPC para registro de novo usuário acadêmico.
    /// </summary>
    public override async Task<UserResponse> Register(RegisterUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[UserGrpcServiceImpl.Register] Recebida solicitação gRPC de registro para: {Matricula}, {Email}", request.Matricula, request.Email);

        try
        {
            var user = await _userService.RegisterAsync(
                request.Matricula,
                request.Nome,
                request.Email,
                request.Senha,
                request.TipoVinculo,
                context.CancellationToken);

            return new UserResponse
            {
                UserId = user.Id.ToString(),
                Matricula = user.Matricula,
                Nome = user.Nome,
                Email = user.Email,
                TipoVinculo = user.TipoVinculo,
                Ativo = user.Ativo
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "[UserGrpcServiceImpl.Register] Conflito de negócio no cadastro: {Message}", ex.Message);
            throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserGrpcServiceImpl.Register] Falha inesperada durante registro");
            throw new RpcException(new Status(StatusCode.Internal, "Erro ao registrar usuário."));
        }
    }
}

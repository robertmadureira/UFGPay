using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UfgPay.Gateway.Application.DTOs;
using UfgPay.Gateway.Application.Interfaces;
using UfgPay.UserService.Grpc;
using UfgPay.WalletService.Grpc;

namespace UfgPay.Gateway.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly UserGrpcService.UserGrpcServiceClient _userClient;
    private readonly WalletGrpcService.WalletGrpcServiceClient _walletClient;
    private readonly IJwtService _jwtService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserGrpcService.UserGrpcServiceClient userClient,
        WalletGrpcService.WalletGrpcServiceClient walletClient,
        IJwtService jwtService,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator,
        ILogger<AuthController> logger)
    {
        _userClient = userClient;
        _walletClient = walletClient;
        _jwtService = jwtService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _logger = logger;
    }

    /// <summary>
    /// Autentica o usuário via credenciais institucionais e retorna token JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("[AuthController.Login] Tentativa de login para matrícula: {Matricula}", request.Matricula);

        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("[AuthController.Login] Validação falhou para matrícula: {Matricula}", request.Matricula);
            return BadRequest(new { sucesso = false, erros = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var authResponse = await _userClient.AuthenticateAsync(new AuthRequest
        {
            Matricula = request.Matricula,
            Senha = request.Senha
        });

        if (!authResponse.Sucesso || !Guid.TryParse(authResponse.UserId, out var userId))
        {
            _logger.LogWarning("[AuthController.Login] Falha de autenticação gRPC para matrícula: {Matricula}", request.Matricula);
            return Unauthorized(new AuthResponseDto(false, string.Empty, Mensagem: authResponse.ErroMensagem));
        }

        var token = _jwtService.GenerateToken(
            userId,
            request.Matricula,
            authResponse.Nome,
            authResponse.Email,
            authResponse.TipoVinculo);

        _logger.LogInformation("[AuthController.Login] Login com sucesso para UserId: {UserId}", userId);

        return Ok(new AuthResponseDto(
            Sucesso: true,
            Token: token,
            UserId: userId.ToString(),
            Nome: authResponse.Nome,
            Email: authResponse.Email,
            TipoVinculo: authResponse.TipoVinculo
        ));
    }

    /// <summary>
    /// Cadastra um novo usuário acadêmico e inicializa sua carteira digital.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("[AuthController.Register] Registrando usuário. Matrícula: {Matricula}, Email: {Email}", request.Matricula, request.Email);

        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("[AuthController.Register] Validação falhou: {@Errors}", validationResult.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new { sucesso = false, erros = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            // 1. Cadastra no UserService via gRPC
            var userResponse = await _userClient.RegisterAsync(new RegisterUserRequest
            {
                Matricula = request.Matricula,
                Nome = request.Nome,
                Email = request.Email,
                Senha = request.Senha,
                TipoVinculo = request.TipoVinculo
            });

            var userId = Guid.Parse(userResponse.UserId);

            // 2. Cria a carteira vinculada no WalletService via gRPC
            _logger.LogInformation("[AuthController.Register] Inicializando carteira para novo UserId: {UserId}", userId);
            await _walletClient.CreateAccountAsync(new CreateAccountRequest { UserId = userId.ToString() });

            // 3. Emite o token JWT inicial
            var token = _jwtService.GenerateToken(
                userId,
                userResponse.Matricula,
                userResponse.Nome,
                userResponse.Email,
                userResponse.TipoVinculo);

            _logger.LogInformation("[AuthController.Register] Registro e carteira concluídos com sucesso para UserId: {UserId}", userId);

            return StatusCode(StatusCodes.Status201Created, new AuthResponseDto(
                Sucesso: true,
                Token: token,
                UserId: userId.ToString(),
                Nome: userResponse.Nome,
                Email: userResponse.Email,
                TipoVinculo: userResponse.TipoVinculo,
                Mensagem: "Usuário registrado e carteira criada com sucesso."
            ));
        }
        catch (Grpc.Core.RpcException ex)
        {
            _logger.LogError(ex, "[AuthController.Register] Erro gRPC durante registro");
            return BadRequest(new { sucesso = false, mensagem = ex.Status.Detail });
        }
    }
}

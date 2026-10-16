using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UfgPay.Gateway.Application.DTOs;
using UfgPay.Gateway.Application.Interfaces;
using UfgPay.WalletService.Grpc;

namespace UfgPay.Gateway.Controllers;

[ApiController]
[Route("api/v1/wallet")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly WalletGrpcService.WalletGrpcServiceClient _walletClient;
    private readonly ICacheService _cacheService;
    private readonly IValidator<TopupPixRequest> _topupValidator;
    private readonly ILogger<WalletController> _logger;

    public WalletController(
        WalletGrpcService.WalletGrpcServiceClient walletClient,
        ICacheService cacheService,
        IValidator<TopupPixRequest> topupValidator,
        ILogger<WalletController> logger)
    {
        _walletClient = walletClient;
        _cacheService = cacheService;
        _topupValidator = topupValidator;
        _logger = logger;
    }

    /// <summary>
    /// Consulta o saldo atual da carteira digital do usuário autenticado (com cache Redis).
    /// </summary>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(BalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBalance()
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized();

        _logger.LogInformation("[WalletController.GetBalance] Consultando saldo para UserId: {UserId}", userId);

        string cacheKey = $"ufgpay:wallet:balance:{userId}";

        // Estratégia de Cache: Rápido (TTL 60s)
        var cachedBalance = await _cacheService.GetAsync<BalanceDto>(cacheKey);
        if (cachedBalance != null)
        {
            _logger.LogDebug("[WalletController.GetBalance] Retornando saldo do cache para UserId: {UserId}", userId);
            return Ok(cachedBalance);
        }

        // Cache miss: consulta gRPC
        var balanceResponse = await _walletClient.GetBalanceAsync(new GetBalanceRequest { UserId = userId.ToString() });

        var balanceDto = new BalanceDto(
            userId,
            balanceResponse.SaldoCentavos,
            balanceResponse.SaldoCentavos / 100.0m,
            balanceResponse.Bloqueada
        );

        // Grava no Redis com TTL de 60 segundos
        await _cacheService.SetAsync(cacheKey, balanceDto, TimeSpan.FromSeconds(60));

        return Ok(balanceDto);
    }

    /// <summary>
    /// Realiza a recarga de saldo via PIX simulado.
    /// </summary>
    [HttpPost("topup-pix")]
    [ProducesResponseType(typeof(TopupPixResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TopupPix([FromBody] TopupPixRequest request)
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized();

        _logger.LogInformation("[WalletController.TopupPix] Solicitação de recarga PIX. UserId: {UserId}, ValorCentavos: {ValorCentavos}",
            userId, request.ValorCentavos);

        var validationResult = await _topupValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("[WalletController.TopupPix] Falha na validação do valor de recarga");
            return BadRequest(new { sucesso = false, erros = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        string chavePix = string.IsNullOrWhiteSpace(request.ChavePix) ? Guid.NewGuid().ToString("N") : request.ChavePix;

        var depositResponse = await _walletClient.DepositPixAsync(new DepositPixRequest
        {
            UserId = userId.ToString(),
            ValorCentavos = request.ValorCentavos,
            ChaveTransacao = chavePix
        });

        if (!depositResponse.Sucesso)
        {
            _logger.LogWarning("[WalletController.TopupPix] Falha no depósito via gRPC: {Mensagem}", depositResponse.Mensagem);
            return BadRequest(new { sucesso = false, mensagem = depositResponse.Mensagem });
        }

        // Invalidação Proativa de Cache
        string cacheKey = $"ufgpay:wallet:balance:{userId}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("[WalletController.TopupPix] Recarga concluída. Novo Saldo: {NovoSaldo}", depositResponse.NovoSaldoCentavos);

        return Ok(new TopupPixResponse(
            Sucesso: true,
            Mensagem: "Recarga PIX creditada com sucesso.",
            NovoSaldoCentavos: depositResponse.NovoSaldoCentavos,
            NovoSaldoReais: depositResponse.NovoSaldoCentavos / 100.0m,
            TransacaoId: depositResponse.TransacaoId
        ));
    }

    private bool TryGetAuthenticatedUserId(out Guid userId)
    {
        userId = Guid.Empty;
        if (HttpContext.Items.TryGetValue("UserId", out var item) && item is Guid id)
        {
            userId = id;
            return true;
        }

        var sub = User.FindFirst("sub")?.Value 
               ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(sub, out userId);
    }
}

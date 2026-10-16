using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UfgPay.CampusService.Grpc;
using UfgPay.Gateway.Application.DTOs;
using UfgPay.Gateway.Application.Interfaces;
using UfgPay.WalletService.Grpc;

namespace UfgPay.Gateway.Controllers;

[ApiController]
[Route("api/v1/campus")]
[Authorize]
public class CampusController : ControllerBase
{
    private readonly CampusGrpcService.CampusGrpcServiceClient _campusClient;
    private readonly WalletGrpcService.WalletGrpcServiceClient _walletClient;
    private readonly ICacheService _cacheService;
    private readonly IValidator<CheckoutRequest> _checkoutValidator;
    private readonly ILogger<CampusController> _logger;

    public CampusController(
        CampusGrpcService.CampusGrpcServiceClient campusClient,
        WalletGrpcService.WalletGrpcServiceClient walletClient,
        ICacheService cacheService,
        IValidator<CheckoutRequest> checkoutValidator,
        ILogger<CampusController> logger)
    {
        _campusClient = campusClient;
        _walletClient = walletClient;
        _cacheService = cacheService;
        _checkoutValidator = checkoutValidator;
        _logger = logger;
    }

    /// <summary>
    /// Retorna o catálogo de serviços ativos do campus (com Cache-Aside de 30 minutos).
    /// </summary>
    [HttpGet("services")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CampusServiceItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices()
    {
        _logger.LogInformation("[CampusController.GetServices] Consultando catálogo de serviços");

        string cacheKey = "ufgpay:campus:services";

        // 1. Tenta recuperar do Redis
        var cached = await _cacheService.GetAsync<List<CampusServiceItemDto>>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("[CampusController.GetServices] Retornando catálogo a partir do cache Redis");
            return Ok(cached);
        }

        // 2. Cache miss: busca via gRPC no CampusService
        var response = await _campusClient.ListServicesAsync(new EmptyRequest());

        var result = response.Servicos.Select(s => new CampusServiceItemDto(
            s.Id,
            s.EstabelecimentoId,
            s.Nome,
            s.PrecoCentavos,
            s.PrecoCentavos / 100.0m
        )).ToList();

        // 3. Salva no cache com TTL de 30 minutos
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

        return Ok(result);
    }

    /// <summary>
    /// Orquestração de Checkout e Emissão de Voucher de Consumo.
    /// </summary>
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(CheckoutResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (!TryGetAuthenticatedUserId(out var userId))
            return Unauthorized();

        _logger.LogInformation("[CampusController.Checkout] Orquestrando checkout. UserId: {UserId}, ServicoId: {ServicoId}, ValorCentavos: {ValorCentavos}",
            userId, request.ServicoId, request.ValorCentavos);

        var validationResult = await _checkoutValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("[CampusController.Checkout] Validação do checkout falhou");
            return BadRequest(new { sucesso = false, erros = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        // Passo 1: Debitar da Carteira
        _logger.LogInformation("[CampusController.Checkout] Passo 1: Solicitando débito de {Valor} centavos ao WalletService", request.ValorCentavos);
        var debitResponse = await _walletClient.DebitBalanceAsync(new DebitBalanceRequest
        {
            UserId = userId.ToString(),
            ValorCentavos = request.ValorCentavos,
            Descricao = $"Consumo no Campus (Serviço: {request.ServicoId})"
        });

        if (!debitResponse.Sucesso)
        {
            _logger.LogWarning("[CampusController.Checkout] Falha ao debitar saldo para UserId: {UserId}. Motivo: {Mensagem}", userId, debitResponse.Mensagem);
            return BadRequest(new CheckoutResponse(
                Sucesso: false,
                NovoSaldoCentavos: debitResponse.NovoSaldoCentavos,
                NovoSaldoReais: debitResponse.NovoSaldoCentavos / 100.0m,
                Mensagem: debitResponse.Mensagem ?? "Saldo insuficiente para realizar a compra."
            ));
        }

        // Passo 2: Processar Pedido e Emitir Voucher
        _logger.LogInformation("[CampusController.Checkout] Passo 2: Emitindo voucher junto ao CampusService");
        var orderResponse = await _campusClient.ProcessOrderAsync(new ProcessOrderRequest
        {
            UserId = userId.ToString(),
            ServicoId = request.ServicoId,
            ValorCentavos = request.ValorCentavos
        });

        if (!orderResponse.Sucesso)
        {
            _logger.LogError("[CampusController.Checkout] Falha crítica na geração do voucher após débito. Realizando estorno compensatório.");
            // Estorno de compensação caso o CampusService falhe
            await _walletClient.DepositPixAsync(new DepositPixRequest
            {
                UserId = userId.ToString(),
                ValorCentavos = request.ValorCentavos,
                ChaveTransacao = $"ESTORNO-{Guid.NewGuid():N}"
            });

            return BadRequest(new CheckoutResponse(
                Sucesso: false,
                Mensagem: "Não foi possível emitir o voucher. O valor foi estornado à sua carteira."
            ));
        }

        // Passo 3: Limpar cache de saldo no Redis
        string balanceCacheKey = $"ufgpay:wallet:balance:{userId}";
        await _cacheService.RemoveAsync(balanceCacheKey);

        _logger.LogInformation("[CampusController.Checkout] Checkout concluído com sucesso! Voucher: {Voucher}", orderResponse.CodigoVoucher);

        return StatusCode(StatusCodes.Status201Created, new CheckoutResponse(
            Sucesso: true,
            PedidoId: orderResponse.PedidoId,
            CodigoVoucher: orderResponse.CodigoVoucher,
            NovoSaldoCentavos: debitResponse.NovoSaldoCentavos,
            NovoSaldoReais: debitResponse.NovoSaldoCentavos / 100.0m,
            Mensagem: "Compra realizada com sucesso! Apresente o código do voucher no balcão."
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

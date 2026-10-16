using Grpc.Core;
using Microsoft.Extensions.Logging;
using UfgPay.WalletService.Application.Interfaces;
using UfgPay.WalletService.Grpc;

namespace UfgPay.WalletService.Grpc;

/// <summary>
/// Adaptador gRPC para o serviço de Carteira e movimentações financeiras.
/// </summary>
public class WalletGrpcServiceImpl : WalletGrpcService.WalletGrpcServiceBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletGrpcServiceImpl> _logger;

    public WalletGrpcServiceImpl(IWalletService walletService, ILogger<WalletGrpcServiceImpl> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    /// <summary>
    /// RPC de criação de conta inicial.
    /// </summary>
    public override async Task<WalletOperationResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[WalletGrpcServiceImpl.CreateAccount] Solicitação recebida para UserId: {UserId}", request.UserId);

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("[WalletGrpcServiceImpl.CreateAccount] UserId inválido: {UserId}", request.UserId);
            return new WalletOperationResponse { Sucesso = false, Mensagem = "ID de usuário inválido." };
        }

        var result = await _walletService.CreateAccountAsync(userId, context.CancellationToken);

        return new WalletOperationResponse
        {
            Sucesso = result.Sucesso,
            Mensagem = result.Mensagem,
            NovoSaldoCentavos = result.NovoSaldoCentavos
        };
    }

    /// <summary>
    /// RPC de consulta de saldo da carteira.
    /// </summary>
    public override async Task<BalanceResponse> GetBalance(GetBalanceRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[WalletGrpcServiceImpl.GetBalance] Solicitação recebida para UserId: {UserId}", request.UserId);

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("[WalletGrpcServiceImpl.GetBalance] UserId inválido: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ID de usuário inválido."));
        }

        var balance = await _walletService.GetBalanceAsync(userId, context.CancellationToken);
        if (balance == null)
        {
            _logger.LogWarning("[WalletGrpcServiceImpl.GetBalance] Conta não encontrada para UserId: {UserId}", request.UserId);
            return new BalanceResponse
            {
                UserId = request.UserId,
                SaldoCentavos = 0,
                Bloqueada = false
            };
        }

        return new BalanceResponse
        {
            UserId = balance.UserId.ToString(),
            SaldoCentavos = balance.SaldoCentavos,
            Bloqueada = balance.Bloqueada
        };
    }

    /// <summary>
    /// RPC para depósito de saldo via PIX.
    /// </summary>
    public override async Task<WalletOperationResponse> DepositPix(DepositPixRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[WalletGrpcServiceImpl.DepositPix] Solicitação de depósito para UserId: {UserId}, Valor: {Valor}",
            request.UserId, request.ValorCentavos);

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("[WalletGrpcServiceImpl.DepositPix] UserId inválido: {UserId}", request.UserId);
            return new WalletOperationResponse { Sucesso = false, Mensagem = "ID de usuário inválido." };
        }

        var result = await _walletService.DepositPixAsync(userId, request.ValorCentavos, request.ChaveTransacao, context.CancellationToken);

        return new WalletOperationResponse
        {
            Sucesso = result.Sucesso,
            Mensagem = result.Mensagem,
            NovoSaldoCentavos = result.NovoSaldoCentavos,
            TransacaoId = result.TransacaoId?.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// RPC para débito de saldo.
    /// </summary>
    public override async Task<WalletOperationResponse> DebitBalance(DebitBalanceRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[WalletGrpcServiceImpl.DebitBalance] Solicitação de débito para UserId: {UserId}, Valor: {Valor}, Descricao: {Descricao}",
            request.UserId, request.ValorCentavos, request.Descricao);

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogWarning("[WalletGrpcServiceImpl.DebitBalance] UserId inválido: {UserId}", request.UserId);
            return new WalletOperationResponse { Sucesso = false, Mensagem = "ID de usuário inválido." };
        }

        var result = await _walletService.DebitBalanceAsync(userId, request.ValorCentavos, request.Descricao, context.CancellationToken);

        return new WalletOperationResponse
        {
            Sucesso = result.Sucesso,
            Mensagem = result.Mensagem,
            NovoSaldoCentavos = result.NovoSaldoCentavos,
            TransacaoId = result.TransacaoId?.ToString() ?? string.Empty
        };
    }
}

using Microsoft.Extensions.Logging;
using UfgPay.WalletService.Application.DTOs;
using UfgPay.WalletService.Application.Interfaces;
using UfgPay.WalletService.Domain.Entities;
using UfgPay.WalletService.Domain.Interfaces;

namespace UfgPay.WalletService.Application.Services;

/// <summary>
/// Implementação dos casos de uso de Carteira e transações financeiras.
/// </summary>
public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<WalletService> _logger;

    public WalletService(IWalletRepository walletRepository, ILogger<WalletService> logger)
    {
        _walletRepository = walletRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova conta para o usuário com saldo zero.
    /// </summary>
    public async Task<OperationResultDto> CreateAccountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[WalletService.CreateAccountAsync] Inicializando conta para UserId: {UserId}", userId);

        var existing = await _walletRepository.GetAccountByUserIdAsync(userId, cancellationToken);
        if (existing != null)
        {
            _logger.LogWarning("[WalletService.CreateAccountAsync] Conta já existente para UserId: {UserId}", userId);
            return new OperationResultDto(true, "Conta já existe.", existing.SaldoCentavos);
        }

        var account = Account.Create(userId);
        await _walletRepository.AddAccountAsync(account, cancellationToken);
        await _walletRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[WalletService.CreateAccountAsync] Conta criada com sucesso para UserId: {UserId}", userId);
        return new OperationResultDto(true, "Conta criada com sucesso.", 0);
    }

    /// <summary>
    /// Consulta o saldo atual de uma conta.
    /// </summary>
    public async Task<BalanceResultDto?> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[WalletService.GetBalanceAsync] Consultando saldo para UserId: {UserId}", userId);

        var account = await _walletRepository.GetAccountByUserIdAsync(userId, cancellationToken);
        if (account == null)
        {
            _logger.LogWarning("[WalletService.GetBalanceAsync] Conta não encontrada para UserId: {UserId}", userId);
            return null;
        }

        return new BalanceResultDto(account.UserId, account.SaldoCentavos, account.Bloqueada);
    }

    /// <summary>
    /// Realiza depósito via PIX creditando o saldo.
    /// </summary>
    public async Task<OperationResultDto> DepositPixAsync(Guid userId, long valorCentavos, string chaveTransacao, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[WalletService.DepositPixAsync] Iniciando depósito PIX. UserId: {UserId}, ValorCentavos: {ValorCentavos}, Chave: {Chave}",
            userId, valorCentavos, chaveTransacao);

        if (valorCentavos <= 0)
        {
            _logger.LogWarning("[WalletService.DepositPixAsync] Valor inválido para depósito: {ValorCentavos}", valorCentavos);
            return new OperationResultDto(false, "O valor de depósito deve ser maior que zero.", 0);
        }

        var account = await _walletRepository.GetAccountByUserIdForUpdateAsync(userId, cancellationToken);
        if (account == null)
        {
            _logger.LogWarning("[WalletService.DepositPixAsync] Conta não encontrada. Criando nova conta para UserId: {UserId}", userId);
            account = Account.Create(userId);
            await _walletRepository.AddAccountAsync(account, cancellationToken);
        }

        try
        {
            account.Creditar(valorCentavos);

            var transaction = TransactionRecord.Create(
                userId,
                "DEPOSITO_PIX",
                valorCentavos,
                $"Depósito via Pix (Ref: {chaveTransacao})");

            await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            await _walletRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[WalletService.DepositPixAsync] Depósito concluído. UserId: {UserId}, NovoSaldo: {NovoSaldo}, TransacaoId: {TransacaoId}",
                userId, account.SaldoCentavos, transaction.Id);

            return new OperationResultDto(true, "Depósito realizado com sucesso.", account.SaldoCentavos, transaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WalletService.DepositPixAsync] Erro ao processar depósito para UserId: {UserId}", userId);
            return new OperationResultDto(false, ex.Message, account.SaldoCentavos);
        }
    }

    /// <summary>
    /// Realiza o débito de saldo para consumo no campus.
    /// </summary>
    public async Task<OperationResultDto> DebitBalanceAsync(Guid userId, long valorCentavos, string descricao, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[WalletService.DebitBalanceAsync] Iniciando débito. UserId: {UserId}, ValorCentavos: {ValorCentavos}, Descricao: {Descricao}",
            userId, valorCentavos, descricao);

        if (valorCentavos <= 0)
        {
            _logger.LogWarning("[WalletService.DebitBalanceAsync] Valor inválido para débito: {ValorCentavos}", valorCentavos);
            return new OperationResultDto(false, "O valor de débito deve ser maior que zero.", 0);
        }

        var account = await _walletRepository.GetAccountByUserIdForUpdateAsync(userId, cancellationToken);
        if (account == null)
        {
            _logger.LogWarning("[WalletService.DebitBalanceAsync] Conta não encontrada para UserId: {UserId}", userId);
            return new OperationResultDto(false, "Conta não encontrada.", 0);
        }

        try
        {
            account.Debitar(valorCentavos);

            var transaction = TransactionRecord.Create(
                userId,
                "DEBITO_CONSUMO",
                valorCentavos,
                descricao);

            await _walletRepository.AddTransactionAsync(transaction, cancellationToken);
            await _walletRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("[WalletService.DebitBalanceAsync] Débito concluído com sucesso. UserId: {UserId}, NovoSaldo: {NovoSaldo}, TransacaoId: {TransacaoId}",
                userId, account.SaldoCentavos, transaction.Id);

            return new OperationResultDto(true, "Débito realizado com sucesso.", account.SaldoCentavos, transaction.Id);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "[WalletService.DebitBalanceAsync] Regra de negócio violada no débito: {Message}", ex.Message);
            return new OperationResultDto(false, ex.Message, account.SaldoCentavos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WalletService.DebitBalanceAsync] Falha inesperada durante débito para UserId: {UserId}", userId);
            return new OperationResultDto(false, "Falha interna ao processar débito.", account.SaldoCentavos);
        }
    }
}

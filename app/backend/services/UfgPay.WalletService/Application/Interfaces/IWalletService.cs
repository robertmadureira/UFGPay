using UfgPay.WalletService.Application.DTOs;

namespace UfgPay.WalletService.Application.Interfaces;

/// <summary>
/// Contrato do serviço de aplicação para operações de saldo, débito e depósitos.
/// </summary>
public interface IWalletService
{
    Task<OperationResultDto> CreateAccountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<BalanceResultDto?> GetBalanceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DepositPixAsync(Guid userId, long valorCentavos, string chaveTransacao, CancellationToken cancellationToken = default);
    Task<OperationResultDto> DebitBalanceAsync(Guid userId, long valorCentavos, string descricao, CancellationToken cancellationToken = default);
}

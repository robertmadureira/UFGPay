using UfgPay.WalletService.Domain.Entities;

namespace UfgPay.WalletService.Domain.Interfaces;

/// <summary>
/// Contrato de repositório para acesso a dados de carteira e extratos (DIP).
/// </summary>
public interface IWalletRepository
{
    Task<Account?> GetAccountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Account?> GetAccountByUserIdForUpdateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAccountAsync(Account account, CancellationToken cancellationToken = default);
    Task AddTransactionAsync(TransactionRecord transaction, CancellationToken cancellationToken = default);
    Task<List<TransactionRecord>> GetTransactionsByUserIdAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UfgPay.WalletService.Domain.Entities;
using UfgPay.WalletService.Domain.Interfaces;
using UfgPay.WalletService.Infrastructure.Data;

namespace UfgPay.WalletService.Infrastructure.Repositories;

/// <summary>
/// Repositório para acesso aos dados da carteira com EF Core.
/// </summary>
public class WalletRepository : IWalletRepository
{
    private readonly WalletDbContext _context;
    private readonly ILogger<WalletRepository> _logger;

    public WalletRepository(WalletDbContext context, ILogger<WalletRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Account?> GetAccountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[WalletRepository.GetAccountByUserIdAsync] Querying account for UserId: {UserId}", userId);
        return await _context.Contas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Account?> GetAccountByUserIdForUpdateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[WalletRepository.GetAccountByUserIdForUpdateAsync] Querying account for update for UserId: {UserId}", userId);
        return await _context.Contas
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task AddAccountAsync(Account account, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[WalletRepository.AddAccountAsync] Adding new account for UserId: {UserId}", account.UserId);
        await _context.Contas.AddAsync(account, cancellationToken);
    }

    public async Task AddTransactionAsync(TransactionRecord transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[WalletRepository.AddTransactionAsync] Adding transaction record: {TransactionId}", transaction.Id);
        await _context.Transacoes.AddAsync(transaction, cancellationToken);
    }

    public async Task<List<TransactionRecord>> GetTransactionsByUserIdAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[WalletRepository.GetTransactionsByUserIdAsync] Querying transactions for UserId: {UserId}, Limit: {Limit}", userId, limit);
        return await _context.Transacoes
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CriadoEm)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[WalletRepository.SaveChangesAsync] Committing changes to PostgreSQL");
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

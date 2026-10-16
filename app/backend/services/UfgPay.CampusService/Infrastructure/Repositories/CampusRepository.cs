using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UfgPay.CampusService.Domain.Entities;
using UfgPay.CampusService.Domain.Interfaces;
using UfgPay.CampusService.Infrastructure.Data;

namespace UfgPay.CampusService.Infrastructure.Repositories;

/// <summary>
/// Repositório para acesso aos dados do campus via EF Core.
/// </summary>
public class CampusRepository : ICampusRepository
{
    private readonly CampusDbContext _context;
    private readonly ILogger<CampusRepository> _logger;

    public CampusRepository(CampusDbContext context, ILogger<CampusRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<CampusServiceItem>> GetActiveServicesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CampusRepository.GetActiveServicesAsync] Querying active campus services");
        return await _context.ServicosCampus
            .AsNoTracking()
            .Where(s => s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<CampusServiceItem?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CampusRepository.GetServiceByIdAsync] Querying service by Id: {ServiceId}", id);
        return await _context.ServicosCampus
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddOrderAsync(ConsumptionOrder order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CampusRepository.AddOrderAsync] Adding consumption order: {OrderId}", order.Id);
        await _context.PedidosConsumo.AddAsync(order, cancellationToken);
    }

    public async Task<ConsumptionOrder?> GetOrderByVoucherAsync(string codigoVoucher, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CampusRepository.GetOrderByVoucherAsync] Querying order by Voucher: {Voucher}", codigoVoucher);
        return await _context.PedidosConsumo
            .AsNoTracking()
            .Include(o => o.Servico)
            .FirstOrDefaultAsync(o => o.CodigoVoucher == codigoVoucher, cancellationToken);
    }

    public async Task<List<ConsumptionOrder>> GetOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CampusRepository.GetOrdersByUserIdAsync] Querying orders for UserId: {UserId}", userId);
        return await _context.PedidosConsumo
            .AsNoTracking()
            .Include(o => o.Servico)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("[CampusRepository.SaveChangesAsync] Committing changes to PostgreSQL database");
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

using UfgPay.CampusService.Domain.Entities;

namespace UfgPay.CampusService.Domain.Interfaces;

/// <summary>
/// Contrato do repositório para acesso a dados do campus (DIP).
/// </summary>
public interface ICampusRepository
{
    Task<List<CampusServiceItem>> GetActiveServicesAsync(CancellationToken cancellationToken = default);
    Task<CampusServiceItem?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddOrderAsync(ConsumptionOrder order, CancellationToken cancellationToken = default);
    Task<ConsumptionOrder?> GetOrderByVoucherAsync(string codigoVoucher, CancellationToken cancellationToken = default);
    Task<List<ConsumptionOrder>> GetOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

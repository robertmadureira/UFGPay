using UfgPay.CampusService.Application.DTOs;

namespace UfgPay.CampusService.Application.Interfaces;

/// <summary>
/// Contrato do serviço de aplicação para catálogo e emissão de vouchers.
/// </summary>
public interface ICampusService
{
    Task<List<CampusServiceModelDto>> ListActiveServicesAsync(CancellationToken cancellationToken = default);
    Task<OrderResultDto> ProcessOrderAsync(Guid userId, Guid servicoId, long valorCentavos, CancellationToken cancellationToken = default);
}

using Microsoft.Extensions.Logging;
using UfgPay.CampusService.Application.DTOs;
using UfgPay.CampusService.Application.Interfaces;
using UfgPay.CampusService.Domain.Entities;
using UfgPay.CampusService.Domain.Interfaces;

namespace UfgPay.CampusService.Application.Services;

/// <summary>
/// Implementação dos casos de uso de Catálogo do Campus e Processamento de Vouchers.
/// </summary>
public class CampusService : ICampusService
{
    private readonly ICampusRepository _campusRepository;
    private readonly ILogger<CampusService> _logger;

    public CampusService(ICampusRepository campusRepository, ILogger<CampusService> logger)
    {
        _campusRepository = campusRepository;
        _logger = logger;
    }

    /// <summary>
    /// Consulta o catálogo de serviços ativos do campus.
    /// </summary>
    public async Task<List<CampusServiceModelDto>> ListActiveServicesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[CampusService.ListActiveServicesAsync] Consultando catálogo de serviços ativos no campus");

        var services = await _campusRepository.GetActiveServicesAsync(cancellationToken);
        _logger.LogInformation("[CampusService.ListActiveServicesAsync] Encontrados {Count} serviços ativos", services.Count);

        return services.Select(s => new CampusServiceModelDto(
            s.Id,
            s.EstabelecimentoId,
            s.Nome,
            s.PrecoCentavos
        )).ToList();
    }

    /// <summary>
    /// Processa o pedido e emite o voucher com código único.
    /// </summary>
    public async Task<OrderResultDto> ProcessOrderAsync(Guid userId, Guid servicoId, long valorCentavos, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[CampusService.ProcessOrderAsync] Processando pedido. UserId: {UserId}, ServicoId: {ServicoId}, ValorCentavos: {ValorCentavos}",
            userId, servicoId, valorCentavos);

        var serviceItem = await _campusRepository.GetServiceByIdAsync(servicoId, cancellationToken);
        if (serviceItem == null || !serviceItem.Ativo)
        {
            _logger.LogWarning("[CampusService.ProcessOrderAsync] Serviço não encontrado ou inativo: {ServicoId}", servicoId);
            return new OrderResultDto(false, Mensagem: "Serviço solicitado não está ativo ou disponível.");
        }

        if (serviceItem.PrecoCentavos != valorCentavos)
        {
            _logger.LogWarning("[CampusService.ProcessOrderAsync] Divergência de valores. Esperado: {Esperado}, Informado: {Informado}",
                serviceItem.PrecoCentavos, valorCentavos);
            return new OrderResultDto(false, Mensagem: "Divergência de preço para o serviço selecionado.");
        }

        var order = ConsumptionOrder.Create(userId, servicoId, valorCentavos);

        await _campusRepository.AddOrderAsync(order, cancellationToken);
        await _campusRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[CampusService.ProcessOrderAsync] Pedido emitido com sucesso! PedidoId: {PedidoId}, Voucher: {Voucher}",
            order.Id, order.CodigoVoucher);

        return new OrderResultDto(
            Sucesso: true,
            PedidoId: order.Id,
            CodigoVoucher: order.CodigoVoucher,
            Mensagem: "Pedido realizado e voucher gerado com sucesso."
        );
    }
}

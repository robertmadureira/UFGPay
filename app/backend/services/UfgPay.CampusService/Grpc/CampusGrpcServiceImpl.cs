using Grpc.Core;
using Microsoft.Extensions.Logging;
using UfgPay.CampusService.Application.Interfaces;
using UfgPay.CampusService.Grpc;

namespace UfgPay.CampusService.Grpc;

/// <summary>
/// Adaptador gRPC para o catálogo do campus e emissão de vouchers.
/// </summary>
public class CampusGrpcServiceImpl : CampusGrpcService.CampusGrpcServiceBase
{
    private readonly ICampusService _campusService;
    private readonly ILogger<CampusGrpcServiceImpl> _logger;

    public CampusGrpcServiceImpl(ICampusService campusService, ILogger<CampusGrpcServiceImpl> logger)
    {
        _campusService = campusService;
        _logger = logger;
    }

    /// <summary>
    /// RPC de listagem de serviços do campus.
    /// </summary>
    public override async Task<ServiceListResponse> ListServices(EmptyRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[CampusGrpcServiceImpl.ListServices] Listando serviços solicitados via gRPC");

        var services = await _campusService.ListActiveServicesAsync(context.CancellationToken);

        var response = new ServiceListResponse();
        foreach (var s in services)
        {
            response.Servicos.Add(new ServiceModel
            {
                Id = s.Id.ToString(),
                EstabelecimentoId = s.EstabelecimentoId,
                Nome = s.Nome,
                PrecoCentavos = s.PrecoCentavos
            });
        }

        return response;
    }

    /// <summary>
    /// RPC para processamento de pedido e emissão de voucher.
    /// </summary>
    public override async Task<OrderResponse> ProcessOrder(ProcessOrderRequest request, ServerCallContext context)
    {
        _logger.LogInformation("[CampusGrpcServiceImpl.ProcessOrder] Solicitação de pedido. UserId: {UserId}, ServicoId: {ServicoId}, Valor: {Valor}",
            request.UserId, request.ServicoId, request.ValorCentavos);

        if (!Guid.TryParse(request.UserId, out var userId) || !Guid.TryParse(request.ServicoId, out var servicoId))
        {
            _logger.LogWarning("[CampusGrpcServiceImpl.ProcessOrder] Identificadores inválidos. UserId: {UserId}, ServicoId: {ServicoId}",
                request.UserId, request.ServicoId);
            return new OrderResponse { Sucesso = false, Mensagem = "Identificadores inválidos." };
        }

        var result = await _campusService.ProcessOrderAsync(userId, servicoId, request.ValorCentavos, context.CancellationToken);

        return new OrderResponse
        {
            Sucesso = result.Sucesso,
            PedidoId = result.PedidoId?.ToString() ?? string.Empty,
            CodigoVoucher = result.CodigoVoucher ?? string.Empty,
            Mensagem = result.Mensagem ?? string.Empty
        };
    }
}

namespace UfgPay.CampusService.Application.DTOs;

public record CampusServiceModelDto(
    Guid Id,
    string EstabelecimentoId,
    string Nome,
    long PrecoCentavos
);

public record OrderResultDto(
    bool Sucesso,
    Guid? PedidoId = null,
    string? CodigoVoucher = null,
    string? Mensagem = null
);

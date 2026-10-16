namespace UfgPay.Gateway.Application.DTOs;

// Auth DTOs
public record LoginRequest(string Matricula, string Senha);

public record RegisterRequest(
    string Matricula,
    string Nome,
    string Email,
    string Senha,
    string TipoVinculo
);

public record AuthResponseDto(
    bool Sucesso,
    string Token,
    string? UserId = null,
    string? Nome = null,
    string? Email = null,
    string? TipoVinculo = null,
    string? Mensagem = null
);

// Wallet DTOs
public record BalanceDto(Guid UserId, long SaldoCentavos, decimal SaldoReais, bool Bloqueada);

public record TopupPixRequest(long ValorCentavos, string? ChavePix = null);

public record TopupPixResponse(bool Sucesso, string Mensagem, long NovoSaldoCentavos, decimal NovoSaldoReais, string? TransacaoId = null);

// Campus DTOs
public record CampusServiceItemDto(string Id, string EstabelecimentoId, string Nome, long PrecoCentavos, decimal PrecoReais);

public record CheckoutRequest(string ServicoId, long ValorCentavos);

public record CheckoutResponse(
    bool Sucesso,
    string? PedidoId = null,
    string? CodigoVoucher = null,
    long? NovoSaldoCentavos = null,
    decimal? NovoSaldoReais = null,
    string? Mensagem = null
);

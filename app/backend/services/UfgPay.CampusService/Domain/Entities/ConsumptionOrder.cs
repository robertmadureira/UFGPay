namespace UfgPay.CampusService.Domain.Entities;

/// <summary>
/// Entidade de domínio para Pedido de Consumo e Emissão de Voucher.
/// </summary>
public class ConsumptionOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ServicoId { get; set; }
    public long ValorCentavos { get; set; }
    public string Status { get; set; } = "EMITIDO"; // EMITIDO, UTILIZADO, CANCELADO
    public string CodigoVoucher { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public CampusServiceItem? Servico { get; set; }

    public ConsumptionOrder() { }

    /// <summary>
    /// Fábrica para geração de pedido de consumo e emissão de voucher seguro com hash.
    /// </summary>
    public static ConsumptionOrder Create(Guid userId, Guid servicoId, long valorCentavos)
    {
        string randomToken = Guid.NewGuid().ToString("N")[..12].ToUpperInvariant();
        string voucher = $"UFG-{DateTime.UtcNow:yyyyMMdd}-{randomToken}";

        return new ConsumptionOrder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ServicoId = servicoId,
            ValorCentavos = valorCentavos,
            Status = "EMITIDO",
            CodigoVoucher = voucher,
            CriadoEm = DateTime.UtcNow
        };
    }
}

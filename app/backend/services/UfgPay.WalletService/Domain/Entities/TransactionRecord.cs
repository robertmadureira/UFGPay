namespace UfgPay.WalletService.Domain.Entities;

/// <summary>
/// Entidade de domínio para registro imutável do extrato financeiro.
/// </summary>
public class TransactionRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Tipo { get; set; } = string.Empty; // DEPOSITO_PIX, DEBITO_CONSUMO
    public long ValorCentavos { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public TransactionRecord() { }

    public static TransactionRecord Create(Guid userId, string tipo, long valorCentavos, string descricao)
    {
        return new TransactionRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Tipo = tipo,
            ValorCentavos = valorCentavos,
            Descricao = descricao,
            CriadoEm = DateTime.UtcNow
        };
    }
}

namespace UfgPay.CampusService.Domain.Entities;

/// <summary>
/// Entidade de domínio para serviço ou produto ofertado no campus.
/// </summary>
public class CampusServiceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EstabelecimentoId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public long PrecoCentavos { get; set; }
    public bool Ativo { get; set; } = true;

    public Establishment? Estabelecimento { get; set; }

    public CampusServiceItem() { }
}

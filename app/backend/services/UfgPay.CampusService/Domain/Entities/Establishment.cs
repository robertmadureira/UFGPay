namespace UfgPay.CampusService.Domain.Entities;

/// <summary>
/// Entidade de domínio para Estabelecimento interno do campus da UFG (RU, Xerox, etc.).
/// </summary>
public class Establishment
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public List<CampusServiceItem> Servicos { get; set; } = new();

    public Establishment() { }
}

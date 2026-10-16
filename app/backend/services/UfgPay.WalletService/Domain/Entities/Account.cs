namespace UfgPay.WalletService.Domain.Entities;

/// <summary>
/// Entidade de domínio representando a Conta/Carteira de um usuário acadêmico.
/// </summary>
public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public long SaldoCentavos { get; set; } = 0;
    public bool Bloqueada { get; set; } = false;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public Account() { }

    /// <summary>
    /// Fábrica para criação de uma nova conta de carteira com saldo zerado.
    /// </summary>
    public static Account Create(Guid userId)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SaldoCentavos = 0,
            Bloqueada = false,
            AtualizadoEm = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Credita valor em centavos na conta.
    /// </summary>
    public void Creditar(long centavos)
    {
        if (centavos <= 0)
            throw new ArgumentException("O valor para crédito deve ser estritamente positivo.", nameof(centavos));

        if (Bloqueada)
            throw new InvalidOperationException("A conta está bloqueada para movimentações.");

        SaldoCentavos += centavos;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Debita valor em centavos da conta, validando saldo suficiente.
    /// </summary>
    public void Debitar(long centavos)
    {
        if (centavos <= 0)
            throw new ArgumentException("O valor para débito deve ser estritamente positivo.", nameof(centavos));

        if (Bloqueada)
            throw new InvalidOperationException("A conta está bloqueada para movimentações.");

        if (SaldoCentavos < centavos)
            throw new InvalidOperationException($"Saldo insuficiente. Saldo atual: {SaldoCentavos} centavos, Solicitado: {centavos} centavos.");

        SaldoCentavos -= centavos;
        AtualizadoEm = DateTime.UtcNow;
    }
}

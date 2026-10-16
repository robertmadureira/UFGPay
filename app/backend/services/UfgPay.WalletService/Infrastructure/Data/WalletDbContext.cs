using Microsoft.EntityFrameworkCore;
using UfgPay.WalletService.Domain.Entities;

namespace UfgPay.WalletService.Infrastructure.Data;

/// <summary>
/// Contexto EF Core para ufg_wallet_db.
/// </summary>
public class WalletDbContext : DbContext
{
    public DbSet<Account> Contas => Set<Account>();
    public DbSet<TransactionRecord> Transacoes => Set<TransactionRecord>();

    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("contas");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.SaldoCentavos).HasColumnName("saldo_centavos").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.Bloqueada).HasColumnName("bloqueada").IsRequired().HasDefaultValue(false);
            entity.Property(e => e.AtualizadoEm).HasColumnName("atualizado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.UserId).IsUnique().HasDatabaseName("idx_contas_user_id");
        });

        modelBuilder.Entity<TransactionRecord>(entity =>
        {
            entity.ToTable("transacoes");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(20).IsRequired();
            entity.Property(e => e.ValorCentavos).HasColumnName("valor_centavos").IsRequired();
            entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(255).IsRequired();
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_transacoes_user_id");
        });
    }
}

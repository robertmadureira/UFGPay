using Microsoft.EntityFrameworkCore;
using UfgPay.CampusService.Domain.Entities;

namespace UfgPay.CampusService.Infrastructure.Data;

/// <summary>
/// Contexto EF Core para ufg_campus_db.
/// </summary>
public class CampusDbContext : DbContext
{
    public DbSet<Establishment> Estabelecimentos => Set<Establishment>();
    public DbSet<CampusServiceItem> ServicosCampus => Set<CampusServiceItem>();
    public DbSet<ConsumptionOrder> PedidosConsumo => Set<ConsumptionOrder>();

    public CampusDbContext(DbContextOptions<CampusDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Establishment>(entity =>
        {
            entity.ToTable("estabelecimentos");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasMaxLength(50);
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Categoria).HasColumnName("categoria").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Ativo).HasColumnName("ativo").IsRequired().HasDefaultValue(true);

            entity.HasMany(e => e.Servicos)
                  .WithOne(s => s.Estabelecimento)
                  .HasForeignKey(s => s.EstabelecimentoId);
        });

        modelBuilder.Entity<CampusServiceItem>(entity =>
        {
            entity.ToTable("servicos_campus");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstabelecimentoId).HasColumnName("estabelecimento_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            entity.Property(e => e.PrecoCentavos).HasColumnName("preco_centavos").IsRequired();
            entity.Property(e => e.Ativo).HasColumnName("ativo").IsRequired().HasDefaultValue(true);
        });

        modelBuilder.Entity<ConsumptionOrder>(entity =>
        {
            entity.ToTable("pedidos_consumo");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.ServicoId).HasColumnName("servico_id").IsRequired();
            entity.Property(e => e.ValorCentavos).HasColumnName("valor_centavos").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(e => e.CodigoVoucher).HasColumnName("codigo_voucher").HasMaxLength(64).IsRequired();
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.UserId).HasDatabaseName("idx_pedidos_user_id");
            entity.HasIndex(e => e.CodigoVoucher).IsUnique().HasDatabaseName("idx_pedidos_voucher");

            entity.HasOne(e => e.Servico)
                  .WithMany()
                  .HasForeignKey(e => e.ServicoId);
        });
    }
}

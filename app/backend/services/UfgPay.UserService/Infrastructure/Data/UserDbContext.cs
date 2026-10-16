using Microsoft.EntityFrameworkCore;
using UfgPay.UserService.Domain.Entities;

namespace UfgPay.UserService.Infrastructure.Data;

/// <summary>
/// Contexto do EF Core para o banco ufg_user_db.
/// </summary>
public class UserDbContext : DbContext
{
    public DbSet<User> Usuarios => Set<User>();

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("usuarios");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Matricula).HasColumnName("matricula").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
            entity.Property(e => e.SenhaHash).HasColumnName("senha_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.TipoVinculo).HasColumnName("tipo_vinculo").HasMaxLength(30).IsRequired();
            entity.Property(e => e.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            entity.Property(e => e.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Matricula).IsUnique().HasDatabaseName("idx_usuarios_matricula");
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("idx_usuarios_email");
        });
    }
}

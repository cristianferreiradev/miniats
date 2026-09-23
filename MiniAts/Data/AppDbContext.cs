// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using MiniAts.Entities;

namespace MiniAts.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Vaga> Vagas => Set<Vaga>();
    public DbSet<Candidatura> Candidaturas => Set<Candidatura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Empresa>(e =>
        {
            e.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            e.Property(x => x.Cnpj).HasMaxLength(14).IsRequired();
            e.HasIndex(x => x.Cnpj).IsUnique();
        });

        modelBuilder.Entity<Vaga>(e =>
        {
            e.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            e.Property(x => x.Descricao).HasMaxLength(4000);
            e.Property(x => x.Salario).HasPrecision(18, 2);

            e.HasOne(x => x.Empresa)
             .WithMany(x => x.Vagas)
             .HasForeignKey(x => x.EmpresaId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.Ativa, x.DataCadastro });
        });

        modelBuilder.Entity<Candidatura>(e =>
        {
            e.Property(x => x.CandidatoNome).HasMaxLength(200).IsRequired();
            e.Property(x => x.CandidatoEmail).HasMaxLength(200).IsRequired();
            e.Property(x => x.Status).HasConversion<int>();

            e.HasOne(x => x.Vaga)
             .WithMany(x => x.Candidaturas)
             .HasForeignKey(x => x.VagaId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.VagaId, x.CandidatoEmail }).IsUnique();
        });
    }
}
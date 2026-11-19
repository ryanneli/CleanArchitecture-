using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dominio.Entities;
using Microsoft.Identity.Client;


namespace Infra.Data.Configurations;

public class Equiapmento : IEntityTypeConfiguration<Equipamento>
{
    public void Configure(EntityTypeBuilder<Equipamento> builder)
    {
        builder.ToTable("Equipamentos");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).IsRequired().HasMaxLength(120);
        builder.Property(e => e.FotoUrl).HasMaxLength(500);
        builder.Property(e => e.Origem).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.FornecedorLocacao).HasMaxLength(150);

        builder.Property(e => e.DataInicioLocacao);
        builder.Property(e => e.DataFimLocacao);
        builder.Property(e => e.CriadoEm).IsRequired();

        builder.HasOne(e => e.Obra).WithMany(e => e.Equipamentos).HasForeignKey(e => e.ObraId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Origem);
        builder.HasIndex(e => e.ObraId);

    }
}


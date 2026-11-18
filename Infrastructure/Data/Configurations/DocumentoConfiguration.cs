using Dominio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Configurations
{
    public sealed class DocumentoConfiguration : IEntityTypeConfiguration<DocumentosObra>
    {
        public void Configure(EntityTypeBuilder<DocumentosObra> builder)
        {
            builder.HasKey(D => D.Id);
            builder.Property(D => D.Sigla).IsRequired().HasMaxLength(10);
            builder.Property(D => D.Nome).IsRequired().HasMaxLength(100);
            builder.Property(D => D.Concluido).IsRequired();

            builder.HasOne(D => D.Obra).
            WithMany(O => O.Documentos).HasForeignKey(D => D.ObraId).OnDelete(DeleteBehavior.Cascade);
            

        }
    }

}
using Dominio.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  public class AppDbContext : IdentityDbContext<Usuarios, IdentityRole<int>, int>
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    
    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Obra> Obras { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    public DbSet<Equipamento> Equipamentos => Set<Equipamento>();
    
    public DbSet<DocumentosObra> DocumentosObras { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);

          modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    }
}
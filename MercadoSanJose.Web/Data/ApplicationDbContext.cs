using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace MercadoSanJose.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Persona> Personas { get; set; }
    public DbSet<Puesto> Puestos { get; set; }
    public DbSet<ConceptoDeuda> ConceptosDeuda { get; set; }
    public DbSet<Deuda> Deudas { get; set; }
    public DbSet<Pago> Pagos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Persona>().ToTable("Persona");
        modelBuilder.Entity<Puesto>().ToTable("Puesto");
        modelBuilder.Entity<ConceptoDeuda>().ToTable("ConceptoDeuda");
        modelBuilder.Entity<Deuda>().ToTable("Deuda");
        modelBuilder.Entity<Pago>().ToTable("Pago");

        modelBuilder.Entity<ConceptoDeuda>().Property(c => c.MontoBase).HasPrecision(10, 2);
        modelBuilder.Entity<Deuda>().Property(d => d.MontoTotal).HasPrecision(10, 2);
        modelBuilder.Entity<Pago>().Property(p => p.MontoPagado).HasPrecision(10, 2);

        modelBuilder.Entity<Puesto>()
            .HasOne(p => p.Inquilino)
            .WithMany(per => per.PuestosComoInquilino)
            .HasForeignKey(p => p.InquilinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Puesto>()
            .HasOne(p => p.Propietario)
            .WithMany(per => per.PuestosComoPropietario)
            .HasForeignKey(p => p.PropietarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
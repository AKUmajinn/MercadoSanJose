using MercadoSanJose.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
        // MAPEO DE NOMBRES DE TABLAS (Para que coincidan con tu SQL)
        modelBuilder.Entity<Persona>().ToTable("Persona");
        modelBuilder.Entity<Puesto>().ToTable("Puesto");
        modelBuilder.Entity<ConceptoDeuda>().ToTable("Concepto_Deuda");
        modelBuilder.Entity<Deuda>().ToTable("Deuda");
        modelBuilder.Entity<Pago>().ToTable("Pago");

        // CONFIGURACIÓN DE DECIMALES (Quita los Warnings amarillos)
        modelBuilder.Entity<ConceptoDeuda>().Property(c => c.MontoBase).HasPrecision(10, 2);
        modelBuilder.Entity<Deuda>().Property(d => d.MontoTotal).HasPrecision(10, 2);
        modelBuilder.Entity<Pago>().Property(p => p.MontoPagado).HasPrecision(10, 2);


        modelBuilder.Entity<Deuda>()
        .Property(d => d.Estado)
        .HasConversion(
            v => v == 1 ? "Pagada" : "Pendiente", // De C# (int) a la DB (string)
            v => v == "Pagada" ? 1 : 0            // De la DB (string) a C# (int)
        );
    }


        // DATA SEMILLA (Opcional, ya que tienes el script SQL)

        //modelBuilder.Entity<Deuda>().Property(d => d.MontoTotal).HasPrecision(18, 2);
        //modelBuilder.Entity<Persona>().HasData(
        //    new Persona { Id = 1, DNI = "00000000", Nombre = "Gerencia/Asociación", EsGerencia = true }
        //);
    
}
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
        modelBuilder.Entity<Deuda>().Property(d => d.MontoTotal).HasPrecision(18, 2);
        modelBuilder.Entity<Persona>().HasData(
            new Persona { Id = 1, DNI = "00000000", Nombre = "Gerencia/Asociación", EsGerencia = true }
        );
    }
}
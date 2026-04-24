using MercadoSanJose.Web.Data; // Obligatorio para ApplicationDbContext
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.DAO;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore; // Obligatorio para UseSqlServer

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración del Contexto de Base de Datos (Esto te faltaba)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPersona, PersonaDAO>();
builder.Services.AddScoped<IPuesto, PuestoDAO>();
builder.Services.AddScoped<IConceptoDeuda, ConceptoDeudaDAO>();
builder.Services.AddScoped<IDeuda, DeudaDAO>();
builder.Services.AddScoped<IPago, PagoDAO>();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
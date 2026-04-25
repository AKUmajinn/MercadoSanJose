using Microsoft.EntityFrameworkCore; // Necesario para AddDbContext
using MercadoSanJose.Web.Data; // Necesario para ApplicationDbContext
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.DAO;
using MercadoSanJose.Web.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la conexión a la base de datos (¡Esto es lo que faltaba!)
var connectionString = builder.Configuration.GetConnectionString("dataBase");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

// 2. Registrar tus DAOs
builder.Services.AddScoped<IPersona, PersonaDAO>();
builder.Services.AddScoped<IPuesto, PuestoDAO>();
builder.Services.AddScoped<IConceptoDeuda, ConceptoDeudaDAO>();
builder.Services.AddScoped<IDeuda, DeudaDAO>();
builder.Services.AddScoped<IPago, PagoDAO>();

var app = builder.Build();

// 3. Configuración del pipeline (Orden correcto)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
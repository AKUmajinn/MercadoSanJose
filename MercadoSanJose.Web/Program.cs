using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.DAO;
using MercadoSanJose.Web.Repositories.Interfaces;
using MercadoSanJose.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("dataBase");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IPersona, PersonaDAO>();
builder.Services.AddScoped<IPuesto, PuestoDAO>();
builder.Services.AddScoped<IConceptoDeuda, ConceptoDeudaDAO>();
builder.Services.AddScoped<IDeuda, DeudaDAO>();
builder.Services.AddScoped<IPago, PagoDAO>();
builder.Services.AddScoped<IDeudaService, DeudaService>();
builder.Services.AddScoped<ICobrosService, CobrosService>();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

// 3. Configuración del pipeline
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
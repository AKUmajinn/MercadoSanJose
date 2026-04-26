using MercadoSanJose.Web.Repositories.DAO;
using MercadoSanJose.Web.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPersona, PersonaDAO>();
builder.Services.AddScoped<IPuesto, PuestoDAO>();
builder.Services.AddScoped<IConceptoDeuda, ConceptoDeudaDAO>();
builder.Services.AddScoped<IDeuda, DeudaDAO>();
builder.Services.AddScoped<IPago, PagoDAO>();
builder.Services.AddScoped<IPersona, PersonaDAO>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
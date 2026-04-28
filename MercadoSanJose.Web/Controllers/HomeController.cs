using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace MercadoSanJose.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inyectamos la base de datos
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
           
            ViewBag.TotalPuestos = await _context.Puestos.CountAsync();

          
            ViewBag.DeudasPendientes = await _context.Deudas
                .Where(d => (int)d.Estado == 0)
                .SumAsync(d => (decimal?)d.MontoTotal) ?? 0m;

            var hoy = DateTime.Today;
            ViewBag.CajaDelDia = await _context.Pagos
                .Where(p => p.FechaPago.Date == hoy)
                .SumAsync(p => (decimal?)p.MontoPagado) ?? 0m;

            
            ViewBag.LocalesAlquilados = await _context.Puestos
                .Where(p => (int)p.Estado == 2)
                .CountAsync();

            return View();
        }
        public async Task<IActionResult> AdminPuestos()
        {
            ViewBag.Personas = await _context.Personas.Where(p => p.Activo).ToListAsync();

           
            var puestos = await _context.Puestos
                .Include(p => p.Propietario)
                .Include(p => p.Inquilino)
                .OrderBy(p => p.NumeroPuesto)
                .ToListAsync();

            return View(puestos);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}
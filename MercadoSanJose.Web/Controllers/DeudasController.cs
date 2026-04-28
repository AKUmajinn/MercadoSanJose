using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models.ViewModels;
using MercadoSanJose.Web.Services;

namespace MercadoSanJose.Web.Controllers;

public class DeudasController : Controller
{
    private readonly IDeudaService _deudaService;
    private readonly ICobrosService _cobrosService;
    private readonly ApplicationDbContext _context;

    public DeudasController(IDeudaService deudaService, ICobrosService cobrosService, ApplicationDbContext context)
    {
        _deudaService = deudaService;
        _cobrosService = cobrosService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(FiltroDeudasVM filtro)
    {
        filtro ??= new FiltroDeudasVM();
        var deudas = await _deudaService.ListarDeudasAsync(filtro);
        ViewBag.Filtro = filtro;
        return View(deudas);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var deuda = await _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (deuda == null) return NotFound();
        return View(deuda);
    }
    [HttpGet]
    public IActionResult GenerarReciboPdf(int deudaId) => Redirect($"/Reportes/GenerarReciboPdf?deudaId={deudaId}");

    [HttpGet]
    public IActionResult DescargarMorosidadExcel() => Redirect("/Reportes/DescargarMorosidadExcel");

    // --- MÉTODOS DE GESTIÓN (Generación y Cobros) ---
    [HttpGet] public async Task<IActionResult> GenerarIndividual() { await CargarSelectListsAsync(); return View(new GenerarDeudaIndividualViewModel { FechaEmision = DateTime.Today }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarIndividual(GenerarDeudaIndividualViewModel model)
    {
        if (!ModelState.IsValid) { await CargarSelectListsAsync(); return View(model); }
        var res = await _deudaService.GenerarDeudaIndividualAsync(model.PuestoId, model.ConceptoDeudaId, model.FechaEmision);
        if (res.Exitoso) { TempData["Success"] = res.Mensaje; TempData["Motivo"] = res.MotivoAsignacion; }
        else TempData["Error"] = res.Mensaje;
        return RedirectToAction(nameof(GenerarIndividual));
    }

    [HttpGet] public async Task<IActionResult> GenerarMasiva() { await CargarSelectListsAsync(); return View(new GenerarDeudaMasivaViewModel { FechaEmision = DateTime.Today }); }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarMasiva(GenerarDeudaMasivaViewModel model)
    {
        if (!ModelState.IsValid) { await CargarSelectListsAsync(); return View(model); }
        var res = await _deudaService.GenerarDeudasMasivasAsync(model.ConceptoDeudaId, model.FechaEmision, model.PuestoIds);
        TempData["ResultadoMasivo"] = System.Text.Json.JsonSerializer.Serialize(res);
        return RedirectToAction(nameof(ResultadoMasivo));
    }

    [HttpGet]
    public IActionResult ResultadoMasivo()
    {
        if (TempData["ResultadoMasivo"] is string json) return View(System.Text.Json.JsonSerializer.Deserialize<ResultadoGeneracionMasiva>(json));
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(string? numeroPuesto, string? dniResponsable)
    {
        var vm = new BuscarDeudasViewModel { NumeroPuesto = numeroPuesto, DNIResponsable = dniResponsable };
        if (!string.IsNullOrWhiteSpace(numeroPuesto) || !string.IsNullOrWhiteSpace(dniResponsable))
            vm.Resultados = (await _deudaService.BuscarDeudasAsync(numeroPuesto, dniResponsable)).ToList();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> RegistrarPago(int deudaId)
    {
        var vm = await _cobrosService.ObtenerDetallePagoAsync(deudaId);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(RegistrarPagoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var res = await _cobrosService.RegistrarPagoAsync(model);
        if (res.Exitoso) { TempData["Success"] = res.Mensaje; return RedirectToAction(nameof(Buscar)); }
        ModelState.AddModelError(string.Empty, res.Mensaje); return View(model);
    }

    [HttpGet] public async Task<IActionResult> Morosidad() => View(await _deudaService.ObtenerMorosidadAsync());

    [HttpGet] public async Task<IActionResult> CajaDiaria(DateTime? fecha) => View(await _deudaService.ObtenerCajaDiariaAsync(fecha ?? DateTime.Today));

    private async Task CargarSelectListsAsync()
    {
        ViewBag.Puestos = new SelectList(await _context.Puestos.OrderBy(p => p.NumeroPuesto).ToListAsync(), "Id", "NumeroPuesto");
        ViewBag.Conceptos = new SelectList(await _context.ConceptosDeuda.OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
    }
}
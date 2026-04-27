using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models.ViewModels;
using MercadoSanJose.Web.Services;


namespace MercadoSanJose.Web.Controllers;

[Authorize]
public class DeudasController : Controller
{
    private readonly IDeudaService _deudaService;
    private readonly ICobrosService _cobrosService;
    private readonly ApplicationDbContext _context;

    public DeudasController(IDeudaService deudaService, ICobrosService cobrosService, ApplicationDbContext context)
    {
        _deudaService  = deudaService;
        _cobrosService = cobrosService;
        _context       = context;
    }

    // ── Index (listado con filtros) ──────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(FiltroDeudasVM filtro)
    {
        filtro ??= new FiltroDeudasVM();
        var deudas = await _deudaService.ListarDeudasAsync(filtro);
        ViewBag.Filtro = filtro;
        return View(deudas);
    }

    // ── Generación individual ────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GenerarIndividual()
    {
        await CargarSelectListsAsync();
        return View(new GenerarDeudaIndividualViewModel { FechaEmision = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarIndividual(GenerarDeudaIndividualViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CargarSelectListsAsync();
            return View(model);
        }

        var resultado = await _deudaService.GenerarDeudaIndividualAsync(
            model.PuestoId, model.ConceptoDeudaId, model.FechaEmision);

        if (resultado.Exitoso)
        {
            TempData["Success"] = resultado.Mensaje;
            TempData["Motivo"]  = resultado.MotivoAsignacion;
        }
        else
        {
            TempData["Error"] = resultado.Mensaje;
        }

        return RedirectToAction(nameof(GenerarIndividual));
    }

    // ── Generación masiva ────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GenerarMasiva()
    {
        await CargarSelectListsAsync();
        return View(new GenerarDeudaMasivaViewModel { FechaEmision = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarMasiva(GenerarDeudaMasivaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CargarSelectListsAsync();
            return View(model);
        }

        var resultado = await _deudaService.GenerarDeudasMasivasAsync(
            model.ConceptoDeudaId, model.FechaEmision, model.PuestoIds);

        TempData["ResultadoMasivo"] = System.Text.Json.JsonSerializer.Serialize(resultado);
        return RedirectToAction(nameof(ResultadoMasivo));
    }

    [HttpGet]
    public IActionResult ResultadoMasivo()
    {
        ResultadoGeneracionMasiva? resultado = null;
        if (TempData["ResultadoMasivo"] is string json)
            resultado = System.Text.Json.JsonSerializer.Deserialize<ResultadoGeneracionMasiva>(json);

        return View(resultado);
    }

    // ── Cobros ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Buscar(string? numeroPuesto, string? dniResponsable)
    {
        var vm = new BuscarDeudasViewModel
        {
            NumeroPuesto  = numeroPuesto,
            DNIResponsable = dniResponsable
        };

        if (!string.IsNullOrWhiteSpace(numeroPuesto) || !string.IsNullOrWhiteSpace(dniResponsable))
            vm.Resultados = (await _deudaService.BuscarDeudasAsync(numeroPuesto, dniResponsable)).ToList();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> RegistrarPago(int deudaId)
    {
        var vm = await _cobrosService.ObtenerDetallePagoAsync(deudaId);
        if (vm is null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(RegistrarPagoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var resultado = await _cobrosService.RegistrarPagoAsync(model);
        if (resultado.Exitoso)
        {
            TempData["Success"] = resultado.Mensaje;
            return RedirectToAction(nameof(Buscar));
        }

        ModelState.AddModelError(string.Empty, resultado.Mensaje);
        return View(model);
    }

    // ── Reportes ─────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Morosidad()
    {
        var deudas = await _deudaService.ObtenerMorosidadAsync();
        return View(deudas);
    }

    [HttpGet]
    public async Task<IActionResult> CajaDiaria(DateTime? fecha)
    {
        var f = fecha ?? DateTime.Today;
        var resumen = await _deudaService.ObtenerCajaDiariaAsync(f);
        return View(resumen);
    }

    // ── Helper ───────────────────────────────────────────────────────────────

    private async Task CargarSelectListsAsync()
    {
        ViewBag.Puestos = new SelectList(
            await _context.Puestos.OrderBy(p => p.NumeroPuesto).ToListAsync(),
            "Id", "NumeroPuesto");

        ViewBag.Conceptos = new SelectList(
            await _context.ConceptosDeuda.OrderBy(c => c.Nombre).ToListAsync(),
            "Id", "Nombre");
    }
}

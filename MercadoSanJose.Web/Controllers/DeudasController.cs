using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models.ViewModels;
using MercadoSanJose.Web.Services;
using MercadoSanJose.Web.Repositories.Interfaces;

namespace MercadoSanJose.Web.Controllers;

public class DeudasController : Controller
{
    private readonly IDeudaService _deudaService;
    private readonly ICobrosService _cobrosService;
    private readonly IPuesto _puestoDao;
    private readonly IConceptoDeuda _conceptoDao;
    private readonly IDeuda _deudaDao;

    public DeudasController(
        IDeudaService deudaService,
        ICobrosService cobrosService,
        IPuesto puestoDao,
        IConceptoDeuda conceptoDao,
        IDeuda deudaDao)
    {
        _deudaService = deudaService;
        _cobrosService = cobrosService;
        _puestoDao = puestoDao;
        _conceptoDao = conceptoDao;
        _deudaDao = deudaDao;
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
    public IActionResult Detalle(int id)
    {
        var deuda = _deudaDao.getById(id);
        if (deuda == null) return NotFound();
        return View(deuda);
    }

    [HttpGet]
    public IActionResult GenerarReciboPdf(int deudaId) => Redirect($"/Reportes/GenerarReciboPdf?deudaId={deudaId}");

    [HttpGet]
    public IActionResult DescargarMorosidadExcel() => Redirect("/Reportes/DescargarMorosidadExcel");

    [HttpGet]
    public IActionResult GenerarIndividual()
    {
        CargarSelectLists();
        return View(new GenerarDeudaIndividualViewModel { FechaEmision = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarIndividual(GenerarDeudaIndividualViewModel model)
    {
        if (!ModelState.IsValid)
        {
            CargarSelectLists();
            return View(model);
        }

        var resultado = await _deudaService.GenerarDeudaIndividualAsync(
            model.PuestoId, model.ConceptoDeudaId, model.FechaEmision);

        if (resultado.Exitoso)
        {
            TempData["Success"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, resultado.Mensaje);
        CargarSelectLists();
        return View(model);
    }

    [HttpGet]
    public IActionResult GenerarMasiva()
    {
        CargarSelectLists();
        return View(new GenerarDeudaMasivaViewModel { FechaEmision = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarMasiva(GenerarDeudaMasivaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            CargarSelectLists();
            return View(model);
        }

        var resultado = await _deudaService.GenerarDeudasMasivasAsync(
            model.ConceptoDeudaId, model.FechaEmision, model.PuestoIds);

        return View("ResultadoMasivo", resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(string? numeroPuesto, string? dniResponsable)
    {
        var vm = new BuscarDeudasViewModel
        {
            NumeroPuesto = numeroPuesto?.Trim(),
            DNIResponsable = dniResponsable?.Trim()
        };

        if (!string.IsNullOrWhiteSpace(numeroPuesto) || !string.IsNullOrWhiteSpace(dniResponsable))
        {
            var resultados = await _deudaService.BuscarDeudasAsync(numeroPuesto, dniResponsable);
            vm.Resultados = resultados.ToList();
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> RegistrarPago(int deudaId)
    {
        var vm = await _cobrosService.ObtenerDetallePagoAsync(deudaId);
        if (vm == null) return NotFound();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPago(RegistrarPagoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var res = await _cobrosService.RegistrarPagoAsync(model);
        if (res.Exitoso)
        {
            TempData["Success"] = res.Mensaje;
            return RedirectToAction(nameof(Detalle), new { id = model.DeudaId });
        }

        ModelState.AddModelError(string.Empty, res.Mensaje);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Morosidad()
    {
        return View(await _deudaService.ObtenerMorosidadAsync());
    }

    [HttpGet]
    public async Task<IActionResult> CajaDiaria(DateTime? fecha)
    {
        return View(await _deudaService.ObtenerCajaDiariaAsync(fecha ?? DateTime.Today));
    }

    private void CargarSelectLists()
    {
        var puestos = _puestoDao.getAll().OrderBy(p => p.NumeroPuesto).ToList();
        var conceptos = _conceptoDao.getAll().OrderBy(c => c.Nombre).ToList();

        ViewBag.Puestos = new SelectList(puestos, "Id", "NumeroPuesto");
        ViewBag.Conceptos = new SelectList(conceptos, "Id", "Nombre");
    }
}
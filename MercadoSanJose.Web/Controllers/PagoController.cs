using Microsoft.AspNetCore.Mvc;
using MercadoSanJose.Web.Repositories.Interfaces;
using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPago _pagoService;
        private readonly IDeuda _deudaService;

        public PagoController(IPago pagoService, IDeuda deudaService)
        {
            _pagoService = pagoService;
            _deudaService = deudaService;
        }

        public IActionResult Index(string search)
        {
            var deudas = _deudaService.getAll().Where(d => d.Estado == 0);
            if (!string.IsNullOrEmpty(search))
                deudas = deudas.Where(d => d.PuestoId.ToString().Contains(search));

            return View(deudas.ToList());
        }

        public IActionResult Create(int id)
        {
            var deuda = _deudaService.getById(id);
            return deuda == null ? RedirectToAction("Index") : View(deuda);
        }

        [HttpPost]
        public IActionResult ConfirmarPago(int DeudaId, string NumeroRecibo, decimal MontoPagado)
        {
            var nuevoPago = new Pago { NumeroRecibo = NumeroRecibo, MontoPagado = MontoPagado };
            var resultado = _pagoService.ProcesarPago(nuevoPago, DeudaId);

            if (resultado.Success)
            {
                TempData["ok"] = resultado.Message;
                return RedirectToAction("Index");
            }

            // CORRECCIÓN AQUÍ:
            var deuda = _deudaService.getById(DeudaId);
            if (deuda == null) return RedirectToAction("Index");

            ModelState.AddModelError("", resultado.Message);
            return View("Create", deuda);
        }
    }
}
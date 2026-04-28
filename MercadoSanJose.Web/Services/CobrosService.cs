using Microsoft.EntityFrameworkCore;
using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models.Enums;
using MercadoSanJose.Web.Models.ViewModels;
using MercadoSanJose.Web.Models;

namespace MercadoSanJose.Web.Services;

public class CobrosService : ICobrosService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CobrosService> _logger;

    public CobrosService(ApplicationDbContext context, ILogger<CobrosService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ResultadoCobro> RegistrarPagoAsync(RegistrarPagoViewModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var deuda = await _context.Deudas
                .Include(d => d.Responsable)
                .FirstOrDefaultAsync(d => d.Id == model.DeudaId);

            if (deuda is null)
                return new ResultadoCobro { Exitoso = false, Mensaje = "Deuda no encontrada" };

            if (deuda.Estado == EstadoDeuda.Pagada)
                return new ResultadoCobro { Exitoso = false, Mensaje = "La deuda ya fue pagada" };

            var pago = new Pago
            {
                DeudaId = model.DeudaId,
                FechaPago = model.FechaPago,
                MontoPagado = model.MontoPagado,
                NumeroRecibo = model.NumeroRecibo
            };

            _context.Pagos.Add(pago);
            deuda.Estado = EstadoDeuda.Pagada;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResultadoCobro
            {
                Exitoso = true,
                PagoId = pago.Id,
                NumeroRecibo = pago.NumeroRecibo,
                Mensaje = $"Pago registrado exitosamente. Recibo: {pago.NumeroRecibo}"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error registrando pago para deuda {DeudaId}", model.DeudaId);
            return new ResultadoCobro { Exitoso = false, Mensaje = $"Error interno: {ex.Message}" };
        }
    }

    public async Task<RegistrarPagoViewModel?> ObtenerDetallePagoAsync(int deudaId)
    {
        return await _context.Deudas
            .Include(d => d.Puesto)
            .Include(d => d.ConceptoDeuda)
            .Include(d => d.Responsable)
            .Where(d => d.Id == deudaId)
            .Select(d => new RegistrarPagoViewModel
            {
                DeudaId = d.Id,
                NumeroPuesto = d.Puesto.NumeroPuesto,
                Concepto = d.ConceptoDeuda.Nombre,
                NombreResponsable = d.Responsable.Nombre,
                MontoTotal = d.MontoTotal,
                MontoPagado = d.MontoTotal,
                FechaPago = DateTime.Today
            }).FirstOrDefaultAsync();
    }
}
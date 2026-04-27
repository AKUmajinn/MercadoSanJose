using MercadoSanJose.Web.Data;
using MercadoSanJose.Web.Models;
using MercadoSanJose.Web.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MercadoSanJose.Web.Repositories.DAO
{
    public class PagoDAO : IPago
    {
        private readonly ApplicationDbContext _context;

        public PagoDAO(ApplicationDbContext context) => _context = context;

        public IEnumerable<Pago> getAll()
        {
            return _context.Pagos.ToList();
        }

        public Pago getById(int id)
        {
            return _context.Pagos.Find(id);
        }

        public int add(Pago entidad)
        {
            _context.Pagos.Add(entidad);
            return _context.SaveChanges();
        }

        public int delete(int id)
        {
            var pago = _context.Pagos.Find(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
                return _context.SaveChanges();
            }
            return 0;
        }

        public (bool Success, string Message) ProcesarPago(Pago pago, int deudaId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var deuda = _context.Deudas.Find(deudaId);

                    if (deuda == null) return (false, "Error: La deuda no existe.");
                    if (deuda.Estado == 1) return (false, "Error: Esta deuda ya fue pagada.");

                    // Validación de monto con redondeo a 2 decimales
                    if (Math.Round(pago.MontoPagado, 2) != Math.Round(deuda.MontoTotal, 2))
                    {
                        return (false, $"Error: El monto pagado ({pago.MontoPagado}) no coincide con el total ({deuda.MontoTotal}).");
                    }

                    // Actualizamos estado y asignamos datos
                    deuda.Estado = 1;
                    pago.DeudaId = deudaId;
                    pago.FechaPago = DateTime.Now;

                    _context.Pagos.Add(pago);
                    _context.SaveChanges();

                    transaction.Commit();
                    return (true, "Pago registrado correctamente.");
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    return (false, "Error de BD: " + (ex.InnerException?.Message ?? ex.Message));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, "Error interno: " + ex.Message);
                }
            }
        }
    }
}
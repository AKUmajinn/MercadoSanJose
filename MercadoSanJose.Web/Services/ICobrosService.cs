using MercadoSanJose.Web.Models.ViewModels;

namespace MercadoSanJose.Web.Services;

public interface ICobrosService
{
    Task<ResultadoCobro> RegistrarPagoAsync(RegistrarPagoViewModel model);
    Task<RegistrarPagoViewModel?> ObtenerDetallePagoAsync(int deudaId);
}

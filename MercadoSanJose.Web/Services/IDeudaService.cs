using MercadoSanJose.Web.Models.ViewModels;

namespace MercadoSanJose.Web.Services;

public interface IDeudaService
{
    
    Task<ResultadoGeneracionDeuda> GenerarDeudaIndividualAsync(int puestoId, int conceptoDeudaId, DateTime fechaEmision);

    
    Task<ResultadoGeneracionMasiva> GenerarDeudasMasivasAsync(int conceptoDeudaId, DateTime fechaEmision, int[]? puestoIds = null);

    Task<IEnumerable<DeudaViewModel>> BuscarDeudasAsync(string? numeroPuesto, string? dniResponsable);
    Task<IEnumerable<DeudaViewModel>> ObtenerMorosidadAsync();
    Task<ResumenCajaDiariaViewModel> ObtenerCajaDiariaAsync(DateTime fecha);

    
    Task<List<DeudaListItemVM>> ListarDeudasAsync(FiltroDeudasVM filtro);
}

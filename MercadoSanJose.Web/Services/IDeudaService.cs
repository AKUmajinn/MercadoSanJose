using MercadoSanJose.Web.Models.ViewModels;

namespace MercadoSanJose.Web.Services;

public interface IDeudaService
{
    /// <summary>
    /// Genera una deuda individual para un puesto específico.
    /// Aplica RF-05: asigna el responsable según el estado del puesto.
    /// </summary>
    Task<ResultadoGeneracionDeuda> GenerarDeudaIndividualAsync(int puestoId, int conceptoDeudaId, DateTime fechaEmision);

    /// <summary>
    /// Generación masiva de deudas para TODOS los puestos activos.
    /// Aplica RF-05 a cada puesto de forma independiente.
    /// </summary>
    Task<ResultadoGeneracionMasiva> GenerarDeudasMasivasAsync(int conceptoDeudaId, DateTime fechaEmision, int[]? puestoIds = null);

    Task<IEnumerable<DeudaViewModel>> BuscarDeudasAsync(string? numeroPuesto, string? dniResponsable);
    Task<IEnumerable<DeudaViewModel>> ObtenerMorosidadAsync();
    Task<ResumenCajaDiariaViewModel> ObtenerCajaDiariaAsync(DateTime fecha);

    /// <summary>
    /// Listado de deudas para la vista Index, con filtros opcionales.
    /// </summary>
    Task<List<DeudaListItemVM>> ListarDeudasAsync(FiltroDeudasVM filtro);
}

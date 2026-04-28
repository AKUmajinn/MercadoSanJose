using System.ComponentModel.DataAnnotations;
using MercadoSanJose.Web.Models.Enums;

namespace MercadoSanJose.Web.Models.ViewModels;

// ─── Resultados de generación ────────────────────────────────────────────────

public class ResultadoGeneracionDeuda
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int? DeudaId { get; set; }
    public string? ResponsableAsignado { get; set; }
    public string? MotivoAsignacion { get; set; }
}

public class ResultadoGeneracionMasiva
{
    public int TotalProcesados { get; set; }
    public int TotalExitosos { get; set; }
    public int TotalErrores { get; set; }
    public decimal MontoTotalGenerado { get; set; }
    public List<ResultadoGeneracionDeuda> Detalles { get; set; } = new();
}

// ─── ViewModels para listados ─────────────────────────────────────────────────

public class DeudaViewModel
{
    public int Id { get; set; }
    public string NumeroPuesto { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string EstadoPuesto { get; set; } = string.Empty;
    public string ConceptoDeuda { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string NombreResponsable { get; set; } = string.Empty;
    public string DNIResponsable { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public EstadoDeuda Estado { get; set; }
}

public class ResumenCajaDiariaViewModel
{
    public DateTime Fecha { get; set; }
    public decimal TotalRecaudado { get; set; }
    public List<ItemCajaDiaria> DetallesPorConcepto { get; set; } = new();
}

public class ItemCajaDiaria
{
    public string Concepto { get; set; } = string.Empty;
    public int CantidadPagos { get; set; }
    public decimal Total { get; set; }
}

// ─── ViewModels para formularios ──────────────────────────────────────────────

public class GenerarDeudaIndividualViewModel
{
    [Required(ErrorMessage = "Seleccione un puesto")]
    public int PuestoId { get; set; }

    [Required(ErrorMessage = "Seleccione un concepto")]
    public int ConceptoDeudaId { get; set; }

    [Required]
    public DateTime FechaEmision { get; set; } = DateTime.Today;
}

public class GenerarDeudaMasivaViewModel
{
    [Required(ErrorMessage = "Seleccione un concepto")]
    public int ConceptoDeudaId { get; set; }

    [Required]
    public DateTime FechaEmision { get; set; } = DateTime.Today;

    public int[]? PuestoIds { get; set; }
}

public class BuscarDeudasViewModel
{
    public string? NumeroPuesto { get; set; }
    public string? DNIResponsable { get; set; }
    public List<DeudaViewModel> Resultados { get; set; } = new();
}

// ─── Listado / filtros del Index de Deudas ───────────────────────────────────

public class DeudaListItemVM
{
    public int Id { get; set; }
    public string NumeroPuesto { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string NombreConcepto { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string NombreResponsable { get; set; } = string.Empty;
    public string DNIResponsable { get; set; } = string.Empty;
    public bool EsGerenciaResponsable { get; set; }
    public decimal MontoTotal { get; set; }
    public EstadoDeuda Estado { get; set; }
}

public class FiltroDeudasVM
{
    public string? NumeroPuesto { get; set; }
    public string? DNIResponsable { get; set; }
    public EstadoDeuda? Estado { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}

public class RegistrarPagoViewModel
{
    public int DeudaId { get; set; }
    public string NumeroPuesto { get; set; } = string.Empty;
    public string Concepto { get; set; } = string.Empty;
    public string NombreResponsable { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }

    [Required(ErrorMessage = "El número de recibo es obligatorio")]
    [StringLength(50)]
    public string NumeroRecibo { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal MontoPagado { get; set; }

    public DateTime FechaPago { get; set; } = DateTime.Today;
}

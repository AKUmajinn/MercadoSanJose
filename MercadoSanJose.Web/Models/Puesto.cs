using MercadoSanJose.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MercadoSanJose.Web.Models;

public class Puesto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El número de puesto es obligatorio")]
    [StringLength(10)]
    public string NumeroPuesto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El sector es obligatorio")]
    [StringLength(50)]
    public string Sector { get; set; } = string.Empty;
    public int? PropietarioId { get; set; }
    public Persona Propietario { get; set; } = null!;
    public int? InquilinoId { get; set; }
    public EstadoPuesto Estado { get; set; } = EstadoPuesto.Disponible;
    public Persona? Inquilino { get; set; }

    public ICollection<Deuda> Deudas { get; set; } = new List<Deuda>();

}

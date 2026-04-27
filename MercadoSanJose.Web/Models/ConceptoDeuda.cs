using System.ComponentModel.DataAnnotations;

namespace MercadoSanJose.Web.Models;

public class ConceptoDeuda
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal MontoBase { get; set; }

    public ICollection<Deuda> Deudas { get; set; } = new List<Deuda>();
}
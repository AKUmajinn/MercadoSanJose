using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MercadoSanJose.Web.Models
{
    [Table("Pago")]
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        public int DeudaId { get; set; }
        public DateTime FechaPago { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 999999, ErrorMessage = "Monto inválido")]
        public decimal MontoPagado { get; set; }

        [Required(ErrorMessage = "El número de recibo es obligatorio")]
        public string NumeroRecibo { get; set; } = string.Empty;
        public Deuda Deuda { get; set; } = null!;
    }
}
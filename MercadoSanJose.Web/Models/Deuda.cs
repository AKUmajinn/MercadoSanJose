using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MercadoSanJose.Web.Models
{
    [Table("Deuda")]
    public class Deuda
    {
        [Key]
        public int Id { get; set; }
        public int PuestoId { get; set; }
        public int ConceptoDeudaId { get; set; }
        public int ResponsableId { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal MontoTotal { get; set; }
        public int Estado { get; set; }
    }
}
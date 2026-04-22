namespace MercadoSanJose.Web.Models;

public class Deuda
{
    public int Id { get; set; }
    public int PuestoId { get; set; }
    public int ConceptoDeudaId { get; set; }
    public int ResponsableId { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal MontoTotal { get; set; }
    public int Estado { get; set; } // 0: Pendiente, 1: Pagada
}
namespace MercadoSanJose.Web.Models;

public class Pago
{
    public int Id { get; set; }
    public int DeudaId { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal MontoPagado { get; set; }
    public string NumeroRecibo { get; set; } = string.Empty;
}
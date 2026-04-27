namespace MercadoSanJose.Web.Models.ViewModels;

public class ResultadoCobro
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int? PagoId { get; set; }
    public string? NumeroRecibo { get; set; }
}

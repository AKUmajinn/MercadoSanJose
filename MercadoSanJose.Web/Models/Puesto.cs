namespace MercadoSanJose.Web.Models;

public class Puesto
{
    public string NumeroPuesto { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public int? PropietarioId { get; set; }
    public int? InquilinoId { get; set; }
}

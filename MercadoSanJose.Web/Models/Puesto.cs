namespace MercadoSanJose.Web.Models;

public class Puesto
{
    public int Id { get; set; }
    public int NumeroPuesto { get; set; }
    public string Sector { get; set; } = string.Empty;
    public int Estado { get; set; } // 0: Disponible, 1: Vendido, 2: Alquilado
    public int PropietarioId { get; set; }
    public int? InquilinoId { get; set; }
}
namespace MercadoSanJose.Web.Models;

public class Persona
{
    public int Id { get; set; }
    public string DNI { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public bool EsGerencia { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Puesto> PuestosComoPropietario { get; set; } = new List<Puesto>();
    public ICollection<Puesto> PuestosComoInquilino { get; set; } = new List<Puesto>();
    public ICollection<Deuda> DeudasComoResponsable { get; set; } = new List<Deuda>();
}
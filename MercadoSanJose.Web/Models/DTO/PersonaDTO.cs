namespace MercadoSanJose.Web.Models.DTO
{
    public class PersonaDTO
    {
        public string Dni { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool EsGerencia { get; set; }
    }
}

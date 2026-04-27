namespace MercadoSanJose.Web.Models.DTO
{
    public class PuestoDTO
    {
        public int Id { get; set; }
        public string NumeroPuesto { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int PropietarioId { get; set; }
        public string PropietarioNombre { get; set; } = string.Empty;
        public int? InquilinoId { get; set; }
        public string? InquilinoNombre { get; set; }
    }
}

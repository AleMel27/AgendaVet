namespace AgendaVet.Api.DTOs.Response
{
    public class ServicioRespDto
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int DuracionEstimadaMin { get; set; }
        public bool Activo { get; set; }
    }
}

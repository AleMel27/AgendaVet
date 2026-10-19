namespace AgendaVet.Api.DTOs.Response
{
    public class RazaRespDto
    {
        public int IdRaza { get; set; }
        public int IdEspecie { get; set; }
        public string EspecieNombre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}

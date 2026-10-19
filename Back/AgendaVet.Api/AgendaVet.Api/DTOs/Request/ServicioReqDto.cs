using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class ServicioReqDto
    {
        [Required(ErrorMessage = "El nombre del servicio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }

        [Range(5, 480, ErrorMessage = "La duración estimada debe estar entre 5 y 480 minutos.")]
        public int DuracionEstimadaMin { get; set; } = 30;

        public bool Activo { get; set; } = true;
    }
}
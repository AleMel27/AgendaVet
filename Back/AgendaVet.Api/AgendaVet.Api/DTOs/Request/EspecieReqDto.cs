using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class EspecieReqDto
    {
        [Required(ErrorMessage = "El nombre de la especie es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}
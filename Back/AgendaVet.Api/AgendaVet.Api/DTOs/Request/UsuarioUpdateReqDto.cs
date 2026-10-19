using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class UsuarioUpdateReqDto
    {
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        public bool Activo { get; set; }
    }
}

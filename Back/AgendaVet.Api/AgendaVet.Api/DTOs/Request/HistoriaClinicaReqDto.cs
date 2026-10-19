using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class HistoriaClinicaReqDto
    {
        public int? IdCita { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio.")]
        public int IdPaciente { get; set; }

        [Required(ErrorMessage = "El veterinario es obligatorio.")]
        public int IdVeterinario { get; set; }

        [Required(ErrorMessage = "La fecha de atención es obligatoria.")]
        public DateTime FechaAtencion { get; set; }

        [Range(0, 500, ErrorMessage = "El peso debe estar entre 0 y 500 kg.")]
        public decimal? PesoKg { get; set; }

        [Range(30, 45, ErrorMessage = "La temperatura debe estar en un rango válido (°C).")]
        public decimal? Temperatura { get; set; }

        [Range(0, 300, ErrorMessage = "La frecuencia cardíaca debe ser un valor válido.")]
        public int? FrecuenciaCardiaca { get; set; }

        [Required(ErrorMessage = "El diagnóstico es obligatorio.")]
        public string Diagnostico { get; set; } = string.Empty;

        public string? Tratamiento { get; set; }
        public string? Observaciones { get; set; }
    }
}   
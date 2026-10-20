using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Exceptions;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class CitaService : ICitaService
    {
        private readonly AgendaVetDbContext _context;

        public CitaService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CitaRespDto>> ObtenerTodasAsync()
        {
            var query = from c in _context.Citas
                        join pac in _context.Pacientes on c.IdPaciente equals pac.IdPaciente into pacGroup
                        from pac in pacGroup.DefaultIfEmpty()
                        join usu in _context.Usuarios on c.IdVeterinario equals usu.IdUsuario into usuGroup
                        from usu in usuGroup.DefaultIfEmpty()
                        join srv in _context.Servicios on c.IdServicio equals srv.IdServicio into srvGroup
                        from srv in srvGroup.DefaultIfEmpty()
                        select MapearARespDto(c, pac, usu, srv);

            return await query.ToListAsync();
        }

        public async Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, int? idVeterinario = null, string? estado = null)
        {
            var baseQuery = from c in _context.Citas
                            join pac in _context.Pacientes on c.IdPaciente equals pac.IdPaciente into pacGroup
                            from pac in pacGroup.DefaultIfEmpty()
                            join usu in _context.Usuarios on c.IdVeterinario equals usu.IdUsuario into usuGroup
                            from usu in usuGroup.DefaultIfEmpty()
                            join srv in _context.Servicios on c.IdServicio equals srv.IdServicio into srvGroup
                            from srv in srvGroup.DefaultIfEmpty()
                            select new { Cita = c, Paciente = pac, Veterinario = usu, Servicio = srv };

            if (idVeterinario.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Cita.IdVeterinario == idVeterinario.Value);
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                baseQuery = baseQuery.Where(x => x.Cita.Estado == estado);
            }

            int totalRecords = await baseQuery.CountAsync();
            var itemsRaw = await baseQuery
                .OrderByDescending(x => x.Cita.FechaHora)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = itemsRaw.Select(x => MapearARespDto(x.Cita, x.Paciente, x.Veterinario, x.Servicio)).ToList();

            return new PaginatedRespDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Items = items
            };
        }

        public async Task<CitaRespDto?> ObtenerPorIdAsync(int id)
        {
            var item = await (from c in _context.Citas
                              where c.IdCita == id
                              join pac in _context.Pacientes on c.IdPaciente equals pac.IdPaciente into pacGroup
                              from pac in pacGroup.DefaultIfEmpty()
                              join usu in _context.Usuarios on c.IdVeterinario equals usu.IdUsuario into usuGroup
                              from usu in usuGroup.DefaultIfEmpty()
                              join srv in _context.Servicios on c.IdServicio equals srv.IdServicio into srvGroup
                              from srv in srvGroup.DefaultIfEmpty()
                              select new { Cita = c, Paciente = pac, Veterinario = usu, Servicio = srv })
                             .FirstOrDefaultAsync();

            return item == null ? null : MapearARespDto(item.Cita, item.Paciente, item.Veterinario, item.Servicio);
        }

        public async Task<CitaRespDto> CrearAsync(CitaReqDto dto)
        {
            var entidad = new Cita
            {
                IdPaciente = dto.IdPaciente,
                IdVeterinario = dto.IdVeterinario,
                IdServicio = dto.IdServicio,
                FechaHora = AjustarFechaHora(dto.FechaHora),
                Motivo = dto.Motivo,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now
            };

            await ValidarDisponibilidadAsync(entidad);
            _context.Citas.Add(entidad);
            await _context.SaveChangesAsync();

            return (await ObtenerPorIdAsync(entidad.IdCita))!;
        }

        public async Task<CitaRespDto?> ActualizarAsync(int id, CitaUpdateReqDto dto)
        {
            var entidad = await _context.Citas.FindAsync(id);
            if (entidad == null) return null;

            entidad.IdVeterinario = dto.IdVeterinario;
            entidad.IdServicio = dto.IdServicio;
            entidad.FechaHora = AjustarFechaHora(dto.FechaHora);
            entidad.Motivo = dto.Motivo;
            entidad.Estado = dto.Estado;

            await ValidarDisponibilidadAsync(entidad, id);
            await _context.SaveChangesAsync();
            return await ObtenerPorIdAsync(id);
        }

        private async Task ValidarDisponibilidadAsync(Cita cita, int? idCitaIgnorada = null)
        {
            var estado = cita.Estado.Replace(" ", string.Empty);
            if (estado.Equals("Cancelada", StringComparison.OrdinalIgnoreCase)
                || estado.Equals("Completada", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var duracion = await _context.Servicios
                .Where(servicio => servicio.IdServicio == cita.IdServicio)
                .Select(servicio => (int?)servicio.DuracionEstimadaMin)
                .FirstOrDefaultAsync();

            if (!duracion.HasValue)
            {
                throw new BusinessException("El servicio seleccionado no existe.");
            }

            var finCita = cita.FechaHora.AddMinutes(duracion.Value);
            var inicioBusqueda = cita.FechaHora.AddDays(-1);
            var citasExistentes = await (
                from existente in _context.Citas
                join servicio in _context.Servicios on existente.IdServicio equals servicio.IdServicio
                where (existente.IdVeterinario == cita.IdVeterinario || existente.IdPaciente == cita.IdPaciente)
                    && existente.IdCita != idCitaIgnorada
                    && existente.Estado != "Cancelada"
                    && existente.Estado != "Completada"
                    && existente.FechaHora >= inicioBusqueda
                    && existente.FechaHora < finCita
                select new
                {
                    existente.IdVeterinario,
                    existente.IdPaciente,
                    existente.FechaHora,
                    servicio.DuracionEstimadaMin
                }).ToListAsync();

            var conflicto = citasExistentes.FirstOrDefault(existente =>
                existente.FechaHora.AddMinutes(existente.DuracionEstimadaMin) > cita.FechaHora);

            if (conflicto != null)
            {
                var motivo = conflicto.IdVeterinario == cita.IdVeterinario
                    ? "el veterinario"
                    : "la mascota";
                throw new BusinessException($"El horario se cruza con otra cita de {motivo}. Selecciona otro horario.");
            }
        }

        private static DateTime AjustarFechaHora(DateTime fechaHora)
        {
            var horarioPeru = TimeSpan.FromHours(-5);
            var fechaLocal = fechaHora.Kind switch
            {
                DateTimeKind.Utc => new DateTimeOffset(fechaHora).ToOffset(horarioPeru).DateTime,
                DateTimeKind.Local => new DateTimeOffset(fechaHora).ToOffset(horarioPeru).DateTime,
                _ => DateTime.SpecifyKind(fechaHora, DateTimeKind.Unspecified)
            };

            if (fechaLocal.TimeOfDay >= TimeSpan.FromHours(20))
            {
                fechaLocal = fechaLocal.Date.AddDays(1).AddHours(8);
            }

            return new DateTimeOffset(fechaLocal, horarioPeru).UtcDateTime;
        }


        public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var entidad = await _context.Citas.FindAsync(id);
            if (entidad == null) return false;

            entidad.Estado = nuevoEstado switch
            {
                "EnAtencion" => "En Atencion",
                "Pendiente" => "Pendiente",
                "Confirmada" => "Confirmada",
                "Completada" => "Completada",
                "Cancelada" => "Cancelada",
                _ => throw new BusinessException("El estado indicado no es válido.")
            };

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _context.Citas.FindAsync(id);
            if (entidad == null) return false;

            _context.Citas.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }

        private static CitaRespDto MapearARespDto(Cita c, Paciente? pac, Usuario? usu, Servicio? srv) => new()
        {
            IdCita = c.IdCita,
            IdPaciente = c.IdPaciente,
            PacienteNombre = pac != null ? pac.Nombre : string.Empty,
            IdVeterinario = c.IdVeterinario,
            VeterinarioNombre = usu != null ? $"{usu.Nombre} {usu.Apellido}" : string.Empty,
            IdServicio = c.IdServicio,
            ServicioNombre = srv != null ? srv.Nombre : string.Empty,
            FechaHora = DateTime.SpecifyKind(c.FechaHora, DateTimeKind.Utc),
            Motivo = c.Motivo,
            Estado = c.Estado,
            FechaCreacion = c.FechaCreacion
        };
    }
}
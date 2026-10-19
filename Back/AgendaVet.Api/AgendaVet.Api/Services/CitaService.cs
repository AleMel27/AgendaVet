using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
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
                FechaHora = dto.FechaHora,
                Motivo = dto.Motivo,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now
            };

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
            entidad.FechaHora = dto.FechaHora;
            entidad.Motivo = dto.Motivo;
            entidad.Estado = dto.Estado;

            await _context.SaveChangesAsync();
            return await ObtenerPorIdAsync(id);
        }

        public async Task<bool> CambiarEstadoAsync(int id, string nuevoEstado)
        {
            var entidad = await _context.Citas.FindAsync(id);
            if (entidad == null) return false;

            entidad.Estado = nuevoEstado;
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
            FechaHora = c.FechaHora,
            Motivo = c.Motivo,
            Estado = c.Estado,
            FechaCreacion = c.FechaCreacion
        };
    }
}
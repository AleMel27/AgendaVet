using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly AgendaVetDbContext _context;

        public PacienteService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PacienteRespDto>> ObtenerTodosAsync()
        {
            var query = from p in _context.Pacientes
                        join prop in _context.Propietarios on p.IdPropietario equals prop.IdPropietario into propGroup
                        from prop in propGroup.DefaultIfEmpty()
                        join esp in _context.Especies on p.IdEspecie equals esp.IdEspecie into espGroup
                        from esp in espGroup.DefaultIfEmpty()
                        join rz in _context.Razas on p.IdRaza equals rz.IdRaza into rzGroup
                        from rz in rzGroup.DefaultIfEmpty()
                        select MapearARespDto(p, prop, esp, rz);

            return await query.ToListAsync();
        }

        public async Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null)
        {
            var baseQuery = from p in _context.Pacientes
                            join prop in _context.Propietarios on p.IdPropietario equals prop.IdPropietario into propGroup
                            from prop in propGroup.DefaultIfEmpty()
                            join esp in _context.Especies on p.IdEspecie equals esp.IdEspecie into espGroup
                            from esp in espGroup.DefaultIfEmpty()
                            join rz in _context.Razas on p.IdRaza equals rz.IdRaza into rzGroup
                            from rz in rzGroup.DefaultIfEmpty()
                            select new { Paciente = p, Propietario = prop, Especie = esp, Raza = rz };

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                baseQuery = baseQuery.Where(x => x.Paciente.Nombre.Contains(buscar) ||
                                                (x.Propietario != null && x.Propietario.Nombre.Contains(buscar)));
            }

            int totalRecords = await baseQuery.CountAsync();
            var itemsRaw = await baseQuery
                .OrderByDescending(x => x.Paciente.IdPaciente)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = itemsRaw.Select(x => MapearARespDto(x.Paciente, x.Propietario, x.Especie, x.Raza)).ToList();

            return new PaginatedRespDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Items = items
            };
        }

        public async Task<PacienteRespDto?> ObtenerPorIdAsync(int id)
        {
            var item = await (from p in _context.Pacientes
                              where p.IdPaciente == id
                              join prop in _context.Propietarios on p.IdPropietario equals prop.IdPropietario into propGroup
                              from prop in propGroup.DefaultIfEmpty()
                              join esp in _context.Especies on p.IdEspecie equals esp.IdEspecie into espGroup
                              from esp in espGroup.DefaultIfEmpty()
                              join rz in _context.Razas on p.IdRaza equals rz.IdRaza into rzGroup
                              from rz in rzGroup.DefaultIfEmpty()
                              select new { Paciente = p, Propietario = prop, Especie = esp, Raza = rz })
                             .FirstOrDefaultAsync();

            return item == null ? null : MapearARespDto(item.Paciente, item.Propietario, item.Especie, item.Raza);
        }

        public async Task<IEnumerable<PacienteRespDto>> ObtenerPorPropietarioAsync(int idPropietario)
        {
            var query = from p in _context.Pacientes
                        where p.IdPropietario == idPropietario
                        join prop in _context.Propietarios on p.IdPropietario equals prop.IdPropietario into propGroup
                        from prop in propGroup.DefaultIfEmpty()
                        join esp in _context.Especies on p.IdEspecie equals esp.IdEspecie into espGroup
                        from esp in espGroup.DefaultIfEmpty()
                        join rz in _context.Razas on p.IdRaza equals rz.IdRaza into rzGroup
                        from rz in rzGroup.DefaultIfEmpty()
                        select MapearARespDto(p, prop, esp, rz);

            return await query.ToListAsync();
        }

        public async Task<PacienteRespDto> CrearAsync(PacienteReqDto dto)
        {
            var entidad = new Paciente
            {
                IdPropietario = dto.IdPropietario,
                IdEspecie = dto.IdEspecie,
                IdRaza = dto.IdRaza,
                Nombre = dto.Nombre,
                FechaNacimiento = dto.FechaNacimiento,
                Sexo = dto.Sexo,
                Color = dto.Color,
                Esterilizado = dto.Esterilizado,
                Observaciones = dto.Observaciones,
                FechaRegistro = DateTime.Now
            };

            _context.Pacientes.Add(entidad);
            await _context.SaveChangesAsync();

            return (await ObtenerPorIdAsync(entidad.IdPaciente))!;
        }

        public async Task<PacienteRespDto?> ActualizarAsync(int id, PacienteReqDto dto)
        {
            var entidad = await _context.Pacientes.FindAsync(id);
            if (entidad == null) return null;

            entidad.IdPropietario = dto.IdPropietario;
            entidad.IdEspecie = dto.IdEspecie;
            entidad.IdRaza = dto.IdRaza;
            entidad.Nombre = dto.Nombre;
            entidad.FechaNacimiento = dto.FechaNacimiento;
            entidad.Sexo = dto.Sexo;
            entidad.Color = dto.Color;
            entidad.Esterilizado = dto.Esterilizado;
            entidad.Observaciones = dto.Observaciones;

            await _context.SaveChangesAsync();
            return await ObtenerPorIdAsync(id);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _context.Pacientes.FindAsync(id);
            if (entidad == null) return false;

            _context.Pacientes.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }

        private static PacienteRespDto MapearARespDto(Paciente p, Propietario? prop, Especie? esp, Raza? rz) => new()
        {
            IdPaciente = p.IdPaciente,
            IdPropietario = p.IdPropietario,
            PropietarioNombreCompleto = prop != null ? $"{prop.Nombre} {prop.Apellido}" : string.Empty,
            IdEspecie = p.IdEspecie,
            EspecieNombre = esp != null ? esp.Nombre : string.Empty,
            IdRaza = p.IdRaza,
            RazaNombre = rz != null ? rz.Nombre : string.Empty,
            Nombre = p.Nombre,
            FechaNacimiento = p.FechaNacimiento,
            Sexo = p.Sexo,
            Color = p.Color,
            Esterilizado = p.Esterilizado,
            Observaciones = p.Observaciones,
            FechaRegistro = p.FechaRegistro
        };
    }
}
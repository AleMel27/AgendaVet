using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class HistoriaClinicaService : IHistoriaClinicaService
    {
        private readonly AgendaVetDbContext _context;

        public HistoriaClinicaService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HistoriaClinicaRespDto>> ObtenerPorPacienteAsync(int idPaciente)
        {
            // 1. Ejecutamos la consulta trayendo los datos necesarios de la base de datos
            var query = await (from h in _context.HistoriasClinicas
                               join p in _context.Pacientes on h.IdPaciente equals p.IdPaciente
                               join u in _context.Usuarios on h.IdVeterinario equals u.IdUsuario
                               where h.IdPaciente == idPaciente
                               select new { Historia = h, Paciente = p, Veterinario = u })
                                .ToListAsync();

            // 2. Aplicamos el ordenamiento y el mapeo, retornando como IEnumerable
            var resultado = query
                .OrderByDescending(x => x.Historia.FechaAtencion)
                .Select(x => MapearARespDto(x.Historia, x.Paciente, x.Veterinario))
                .ToList();

            return resultado;
        }

        public async Task<PaginatedRespDto> ObtenerPaginadoPorPacienteAsync(int idPaciente, int pageNumber, int pageSize)
        {
            var baseQuery = from h in _context.HistoriasClinicas
                            where h.IdPaciente == idPaciente
                            join pac in _context.Pacientes on h.IdPaciente equals pac.IdPaciente into pacGroup
                            from pac in pacGroup.DefaultIfEmpty()
                            join usu in _context.Usuarios on h.IdVeterinario equals usu.IdUsuario into usuGroup
                            from usu in usuGroup.DefaultIfEmpty()
                            select new { Historia = h, Paciente = pac, Veterinario = usu };

            int totalRecords = await baseQuery.CountAsync();
            var itemsRaw = await baseQuery
                .OrderByDescending(x => x.Historia.FechaAtencion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = itemsRaw.Select(x => MapearARespDto(x.Historia, x.Paciente, x.Veterinario)).ToList();

            return new PaginatedRespDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Items = items
            };
        }

        public async Task<HistoriaClinicaRespDto?> ObtenerPorIdAsync(int id)
        {
            var item = await (from h in _context.HistoriasClinicas
                              where h.IdHistoria == id
                              join pac in _context.Pacientes on h.IdPaciente equals pac.IdPaciente into pacGroup
                              from pac in pacGroup.DefaultIfEmpty()
                              join usu in _context.Usuarios on h.IdVeterinario equals usu.IdUsuario into usuGroup
                              from usu in usuGroup.DefaultIfEmpty()
                              select new { Historia = h, Paciente = pac, Veterinario = usu })
                             .FirstOrDefaultAsync();

            return item == null ? null : MapearARespDto(item.Historia, item.Paciente, item.Veterinario);
        }

        public async Task<HistoriaClinicaRespDto> CrearAsync(HistoriaClinicaReqDto dto)
        {
            var entidad = new HistoriaClinica
            {
                IdCita = dto.IdCita,
                IdPaciente = dto.IdPaciente,
                IdVeterinario = dto.IdVeterinario,
                FechaAtencion = dto.FechaAtencion != default ? dto.FechaAtencion : DateTime.Now,
                PesoKg = dto.PesoKg,
                Temperatura = dto.Temperatura,
                FrecuenciaCardiaca = dto.FrecuenciaCardiaca,
                Diagnostico = dto.Diagnostico,
                Tratamiento = dto.Tratamiento,
                Observaciones = dto.Observaciones
            };

            _context.HistoriasClinicas.Add(entidad);
            await _context.SaveChangesAsync();

            return (await ObtenerPorIdAsync(entidad.IdHistoria))!;
        }

        private static HistoriaClinicaRespDto MapearARespDto(HistoriaClinica h, Paciente? pac, Usuario? usu) => new()
        {
            IdHistoria = h.IdHistoria,
            IdCita = h.IdCita,
            IdPaciente = h.IdPaciente,
            PacienteNombre = pac != null ? pac.Nombre : string.Empty,
            IdVeterinario = h.IdVeterinario,
            VeterinarioNombre = usu != null ? $"{usu.Nombre} {usu.Apellido}" : string.Empty,
            FechaAtencion = h.FechaAtencion,
            PesoKg = h.PesoKg,
            Temperatura = h.Temperatura,
            FrecuenciaCardiaca = h.FrecuenciaCardiaca,
            Diagnostico = h.Diagnostico,
            Tratamiento = h.Tratamiento,
            Observaciones = h.Observaciones
        };
    }
}
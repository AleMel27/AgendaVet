using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class ServicioService : IServicioService
    {
        private readonly AgendaVetDbContext _context;

        public ServicioService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServicioRespDto>> ObtenerTodosAsync(bool soloActivos = true)
        {
            var query = _context.Servicios.AsQueryable();

            if (soloActivos)
            {
                query = query.Where(s => s.Activo);
            }

            return await query
                .Select(s => MapearARespDto(s))
                .ToListAsync();
        }

        public async Task<ServicioRespDto?> ObtenerPorIdAsync(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            return servicio == null ? null : MapearARespDto(servicio);
        }

        public async Task<ServicioRespDto> CrearAsync(ServicioReqDto dto)
        {
            var entidad = new Servicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                DuracionEstimadaMin = dto.DuracionEstimadaMin,
                Activo = dto.Activo
            };

            _context.Servicios.Add(entidad);
            await _context.SaveChangesAsync();

            return MapearARespDto(entidad);
        }

        public async Task<ServicioRespDto?> ActualizarAsync(int id, ServicioReqDto dto)
        {
            var entidad = await _context.Servicios.FindAsync(id);
            if (entidad == null) return null;

            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.Precio = dto.Precio;
            entidad.DuracionEstimadaMin = dto.DuracionEstimadaMin;
            entidad.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return MapearARespDto(entidad);
        }

        private static ServicioRespDto MapearARespDto(Servicio s) => new()
        {
            IdServicio = s.IdServicio,
            Nombre = s.Nombre,
            Descripcion = s.Descripcion,
            Precio = s.Precio,
            DuracionEstimadaMin = s.DuracionEstimadaMin,
            Activo = s.Activo
        };
    }
}
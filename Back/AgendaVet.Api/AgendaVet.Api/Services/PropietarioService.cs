using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class PropietarioService : IPropietarioService
    {
        private readonly AgendaVetDbContext _context;

        public PropietarioService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PropietarioRespDto>> ObtenerTodosAsync()
        {
            return await _context.Propietarios
                .Select(p => MapearARespDto(p))
                .ToListAsync();
        }

        public async Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null)
        {
            var query = _context.Propietarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar) ||
                                         p.Apellido.Contains(buscar) ||
                                         p.DniCe.Contains(buscar));
            }

            int totalRecords = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.IdPropietario)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => MapearARespDto(p))
                .ToListAsync();

            return new PaginatedRespDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Items = items
            };
        }

        public async Task<PropietarioRespDto?> ObtenerPorIdAsync(int id)
        {
            var p = await _context.Propietarios.FindAsync(id);
            return p == null ? null : MapearARespDto(p);
        }

        public async Task<PropietarioRespDto?> ObtenerPorDniAsync(string dniCe)
        {
            var p = await _context.Propietarios.FirstOrDefaultAsync(p => p.DniCe == dniCe);
            return p == null ? null : MapearARespDto(p);
        }

        public async Task<PropietarioRespDto> CrearAsync(PropietarioReqDto dto)
        {
            var entidad = new Propietario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DniCe = dto.DniCe,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                FechaRegistro = DateTime.Now
            };

            _context.Propietarios.Add(entidad);
            await _context.SaveChangesAsync();

            return MapearARespDto(entidad);
        }

        public async Task<PropietarioRespDto?> ActualizarAsync(int id, PropietarioReqDto dto)
        {
            var entidad = await _context.Propietarios.FindAsync(id);
            if (entidad == null) return null;

            entidad.Nombre = dto.Nombre;
            entidad.Apellido = dto.Apellido;
            entidad.DniCe = dto.DniCe;
            entidad.Telefono = dto.Telefono;
            entidad.Email = dto.Email;
            entidad.Direccion = dto.Direccion;

            await _context.SaveChangesAsync();
            return MapearARespDto(entidad);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _context.Propietarios.FindAsync(id);
            if (entidad == null) return false;

            _context.Propietarios.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }

        private static PropietarioRespDto MapearARespDto(Propietario p) => new()
        {
            IdPropietario = p.IdPropietario,
            Nombre = p.Nombre,
            Apellido = p.Apellido,
            DniCe = p.DniCe,
            Telefono = p.Telefono,
            Email = p.Email,
            Direccion = p.Direccion,
            FechaRegistro = p.FechaRegistro
        };
    }
}
using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AgendaVetDbContext _context;

        public UsuarioService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuarioRespDto>> ObtenerTodosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.IdRol)
                .Select(u => MapearARespDto(u))
                .ToListAsync();
        }

        public async Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null)
        {
            var query = _context.Usuarios
                .Include(u => u.IdRol)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                query = query.Where(u => u.Nombre.Contains(buscar) ||
                                         u.Apellido.Contains(buscar) ||
                                         u.Email.Contains(buscar));
            }

            int totalRecords = await query.CountAsync();
            var items = await query
                .OrderByDescending(u => u.IdUsuario)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => MapearARespDto(u))
                .ToListAsync();

            return new PaginatedRespDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                Items = items
            };
        }

        public async Task<UsuarioRespDto?> ObtenerPorIdAsync(int id)
        {
            var u = await _context.Usuarios
                .Include(u => u.IdRol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            return u == null ? null : MapearARespDto(u);
        }

        public async Task<UsuarioRespDto> CrearAsync(UsuarioReqDto dto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var entidad = new Usuario
            {
                IdRol = dto.IdRol,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Password = passwordHash,
                Telefono = dto.Telefono,
                Activo = true,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(entidad);
            await _context.SaveChangesAsync();

            return (await ObtenerPorIdAsync(entidad.IdUsuario))!;
        }

        public async Task<UsuarioRespDto?> ActualizarAsync(int id, UsuarioUpdateReqDto dto)
        {
            var entidad = await _context.Usuarios.FindAsync(id);
            if (entidad == null) return null;

            entidad.IdRol = dto.IdRol;
            entidad.Nombre = dto.Nombre;
            entidad.Apellido = dto.Apellido;
            entidad.Telefono = dto.Telefono;

            await _context.SaveChangesAsync();
            return await ObtenerPorIdAsync(id);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _context.Usuarios.FindAsync(id);
            if (entidad == null) return false;

            entidad.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        private static UsuarioRespDto MapearARespDto(Usuario u) => new()
        {
            IdUsuario = u.IdUsuario,
            IdRol = u.IdRol,
            RolNombre = string.Empty,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Email = u.Email,
            Telefono = u.Telefono,
            Activo = u.Activo,
            FechaRegistro = u.FechaRegistro
        };
    }
}
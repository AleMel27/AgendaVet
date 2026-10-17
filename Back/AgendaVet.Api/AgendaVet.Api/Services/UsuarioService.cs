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
            var query = from u in _context.Usuarios
                        join r in _context.Roles on u.IdRol equals r.IdRol into rolGroup
                        from r in rolGroup.DefaultIfEmpty()
                        select new UsuarioRespDto
                        {
                            IdUsuario = u.IdUsuario,
                            IdRol = u.IdRol,
                            RolNombre = r != null ? r.Nombre : string.Empty,
                            Nombre = u.Nombre,
                            Apellido = u.Apellido,
                            Email = u.Email,
                            Telefono = u.Telefono,
                            Activo = u.Activo,
                            FechaRegistro = u.FechaRegistro
                        };
            return await query.ToListAsync();
        }

        public async Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null)
        {
            var baseQuery = from u in _context.Usuarios
                            join r in _context.Roles on u.IdRol equals r.IdRol into rolGroup
                            from r in rolGroup.DefaultIfEmpty()
                            select new { Usuario = u, Rol = r };

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                baseQuery = baseQuery.Where(x => x.Usuario.Nombre.Contains(buscar) ||
                                                 x.Usuario.Apellido.Contains(buscar) ||
                                                 x.Usuario.Email.Contains(buscar));
            }

            int totalRecords = await baseQuery.CountAsync();
            var itemsRaw = await baseQuery
                .OrderByDescending(x => x.Usuario.IdUsuario)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = itemsRaw.Select(x => new UsuarioRespDto
            {
                IdUsuario = x.Usuario.IdUsuario,
                IdRol = x.Usuario.IdRol,
                RolNombre = x.Rol != null ? x.Rol.Nombre : string.Empty,
                Nombre = x.Usuario.Nombre,
                Apellido = x.Usuario.Apellido,
                Email = x.Usuario.Email,
                Telefono = x.Usuario.Telefono,
                Activo = x.Usuario.Activo,
                FechaRegistro = x.Usuario.FechaRegistro
            }).ToList();

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
            var item = await (from u in _context.Usuarios
                              where u.IdUsuario == id
                              join r in _context.Roles on u.IdRol equals r.IdRol into rolGroup
                              from r in rolGroup.DefaultIfEmpty()
                              select new { Usuario = u, Rol = r })
                             .FirstOrDefaultAsync();

            if (item == null) return null;

            return new UsuarioRespDto
            {
                IdUsuario = item.Usuario.IdUsuario,
                IdRol = item.Usuario.IdRol,
                RolNombre = item.Rol != null ? item.Rol.Nombre : string.Empty,
                Nombre = item.Usuario.Nombre,
                Apellido = item.Usuario.Apellido,
                Email = item.Usuario.Email,
                Telefono = item.Usuario.Telefono,
                Activo = item.Usuario.Activo,
                FechaRegistro = item.Usuario.FechaRegistro
            };
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

    }
}
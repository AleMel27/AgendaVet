using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class RolService : IRolService
    {
        private readonly AgendaVetDbContext _context;

        public RolService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolRespDto>> ObtenerTodosAsync()
        {
            return await _context.Roles
                .Select(r => new RolRespDto
                {
                    IdRol = r.IdRol,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion
                })
                .ToListAsync();
        }

        public async Task<RolRespDto?> ObtenerPorIdAsync(int id)
        {
            var r = await _context.Roles.FindAsync(id);
            return r == null ? null : new RolRespDto
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion
            };
        }
    }
}
using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class RazaService : IRazaService
    {
        private readonly AgendaVetDbContext _context;

        public RazaService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RazaRespDto>> ObtenerTodasAsync()
        {
            var query = from r in _context.Razas
                        join e in _context.Especies on r.IdEspecie equals e.IdEspecie into espGroup
                        from e in espGroup.DefaultIfEmpty()
                        select MapearARespDto(r, e);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<RazaRespDto>> ObtenerPorEspecieAsync(int idEspecie)
        {
            var query = from r in _context.Razas
                        where r.IdEspecie == idEspecie
                        join e in _context.Especies on r.IdEspecie equals e.IdEspecie into espGroup
                        from e in espGroup.DefaultIfEmpty()
                        select MapearARespDto(r, e);

            return await query.ToListAsync();
        }

        public async Task<RazaRespDto?> ObtenerPorIdAsync(int id)
        {
            var item = await (from r in _context.Razas
                              where r.IdRaza == id
                              join e in _context.Especies on r.IdEspecie equals e.IdEspecie into espGroup
                              from e in espGroup.DefaultIfEmpty()
                              select new { Raza = r, Especie = e })
                             .FirstOrDefaultAsync();

            return item == null ? null : MapearARespDto(item.Raza, item.Especie);
        }

        public async Task<RazaRespDto> CrearAsync(RazaReqDto dto)
        {
            var entidad = new Raza
            {
                IdEspecie = dto.IdEspecie,
                Nombre = dto.Nombre
            };

            _context.Razas.Add(entidad);
            await _context.SaveChangesAsync();

            return (await ObtenerPorIdAsync(entidad.IdRaza))!;
        }

        public async Task<RazaRespDto?> ActualizarAsync(int id, RazaReqDto dto)
        {
            var entidad = await _context.Razas.FindAsync(id);
            if (entidad == null) return null;

            entidad.IdEspecie = dto.IdEspecie;
            entidad.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
            return await ObtenerPorIdAsync(id);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _context.Razas.FindAsync(id);
            if (entidad == null) return false;

            _context.Razas.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }

        private static RazaRespDto MapearARespDto(Raza r, Especie? e) => new()
        {
            IdRaza = r.IdRaza,
            IdEspecie = r.IdEspecie,
            EspecieNombre = e != null ? e.Nombre : string.Empty,
            Nombre = r.Nombre
        };
    }
}
using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaVet.Api.Services
{
    public class EspecieService : IEspecieService
    {
        private readonly AgendaVetDbContext _context;

        public EspecieService(AgendaVetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EspecieRespDto>> ObtenerTodasAsync()
        {
            return await _context.Especies
                .Select(e => MapearARespDto(e))
                .ToListAsync();
        }

        public async Task<EspecieRespDto?> ObtenerPorIdAsync(int id)
        {
            var especie = await _context.Especies.FindAsync(id);
            return especie == null ? null : MapearARespDto(especie);
        }

        public async Task<EspecieRespDto> CrearAsync(EspecieReqDto dto)
        {
            var entidad = new Especie
            {
                Nombre = dto.Nombre
            };

            _context.Especies.Add(entidad);
            await _context.SaveChangesAsync();

            return MapearARespDto(entidad);
        }

        public async Task<EspecieRespDto?> ActualizarAsync(int id, EspecieReqDto dto)
        {
            var entidad = await _context.Especies.FindAsync(id);
            if (entidad == null) return null;

            entidad.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
            return MapearARespDto(entidad);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _context.Especies.FindAsync(id);
            if (entidad == null) return false;

            _context.Especies.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }

        private static EspecieRespDto MapearARespDto(Especie e) => new()
        {
            IdEspecie = e.IdEspecie,
            Nombre = e.Nombre
        };
    }
}
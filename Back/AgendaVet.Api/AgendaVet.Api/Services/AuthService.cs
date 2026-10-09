using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;
using AgendaVet.Api.Interfaces;
using AgendaVet.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AgendaVet.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AgendaVetDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AgendaVetDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginRespDto?> LoginAsync(LoginReqDto dto)
        {
            // Busca el usuario directamente por Email
            var usuario = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return null;

            // Validar si el usuario está activo
            if (!usuario.Activo)
                return null;

            // Validación de contraseña encriptada con BCrypt
            bool esPasswordValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Password);
            if (!esPasswordValido)
                return null;

            // Obtener el rol para armar los claims del JWT
            var rol = await _context.Roles.FindAsync(usuario.IdRol);
            string rolNombre = rol?.Nombre ?? "Recepcionista";

            // Generar el token JWT pasando la entidad Usuario completa y el nombre del rol
            string token = _jwtService.GenerarToken(usuario, rolNombre);

            return new LoginRespDto
            {
                Token = token,
                Usuario = new UsuarioRespDto
                {
                    IdUsuario = usuario.IdUsuario,
                    IdRol = usuario.IdRol,
                    RolNombre = rolNombre,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    Telefono = usuario.Telefono,
                    Activo = usuario.Activo,
                    FechaRegistro = usuario.FechaRegistro
                }
            };
        }
    }
}
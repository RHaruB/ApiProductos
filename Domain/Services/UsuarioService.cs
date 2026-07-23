using Application.Contrato;
using Domain.Utils;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly InventarioContext _context;
        private readonly IAesEncryptionService _encripta;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            InventarioContext context, 
            IAesEncryptionService encripta, 
            IJwtService jwtService,
            ILogger<UsuarioService> logger)
        {
            _context = context;
            _encripta = encripta;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<UsuarioResponseDTO> Loggin(string usuario, string contrasena)
        {
            _logger.LogInformation("Intentando iniciar sesión para el usuario: {Usuario}", usuario);

            var usuarioEncontrado = await _context.Usuarios
                                                   .Where(u =>
                                                       u.Usuario1 == usuario &&
                                                       u.PasswordHash == _encripta.Encrypt(contrasena) && 
                                                       u.Activo == true
                                                     ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                _logger.LogWarning("Inicio de sesión fallido para el usuario: {Usuario}. Credenciales incorrectas o usuario inactivo.", usuario);
                return null;
            }

            _logger.LogInformation("Inicio de sesión exitoso para el usuario: {Usuario}. Generando token JWT.", usuario);

            return new UsuarioResponseDTO
            {
                Usuario = usuarioEncontrado.Usuario1,
                Nombre = usuarioEncontrado.Nombre,
                Activo = usuarioEncontrado.Activo,
                Token = _jwtService.GenerateToken(usuarioEncontrado.Id)
            };
        }

        public async Task<(int id, string mensaje)> CrearUsuario(UsuarioDTO usuario)
        {
            _logger.LogInformation("Iniciando creación de usuario con nombre de usuario: {Usuario}", usuario.Usuario);
            try
            {
                string validarionErrores = ValidarUsuarioRequest(usuario);

                if (validarionErrores.Length > 0)
                {
                    _logger.LogWarning("Fallo en la validación del request al crear usuario: {Errores}", validarionErrores);
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarUsuarioExistente(usuario.Usuario);

                if (existeNombre.Length > 0)
                {
                    _logger.LogWarning("Fallo al crear usuario: El nombre de usuario {Usuario} ya existe.", usuario.Usuario);
                    return (0, existeNombre);
                }

                Usuario userDB = new Usuario
                {
                    Nombre = usuario.Nombre,
                    Usuario1 = usuario.Usuario,
                    PasswordHash = _encripta.Encrypt(usuario.PasswordHash),
                    Activo = usuario.Activo,
                    FechaCreacion = DateTime.Now
                };

                await _context.Usuarios.AddAsync(userDB);
                await _context.SaveChangesAsync();  

                int idGenerado = userDB.Id; 
                _logger.LogInformation("Usuario creado correctamente con ID: {UsuarioId}", idGenerado);

                return (idGenerado, "Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear usuario {Usuario}", usuario.Usuario);
                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }
        }

        public static string ValidarUsuarioRequest(UsuarioDTO usuarioRequest, bool editar = false, bool actualizarConstrasena = true)
        {
            var errores = new List<string>();
            if (usuarioRequest == null)
            {
                return "El objeto UsuarioRequest no puede ser nulo";
            }
            if (editar && usuarioRequest.Id <= 0)
            {
                errores.Add("El campo Id es requerido para la edición");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Usuario))
            {
                errores.Add("El campo NombreUsuario es requerido");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.PasswordHash) && actualizarConstrasena)
            {
                errores.Add("El campo Contrasena es requerido");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Nombre))
            {
                errores.Add("El campo Nombre es requerido");
            }
           
            return string.Join(", ", errores);
        }

        private async Task<string> ValidarUsuarioExistente(string Username, int? UsuarioIDExcluir = null)
        {
            try
            {
                var existe = await _context.Usuarios.Where(s => s.Usuario1 == Username).AnyAsync();
                if (existe)
                {
                    return "El usuario ya existe";
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar si el usuario {Usuario} existe en la base de datos.", Username);
                throw;
            }
        }
    }
}

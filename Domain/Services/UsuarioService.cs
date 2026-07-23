using Application.Contrato;
using Domain.Utils;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Services
{
    public class UsuarioService :IUsuarioService
    {
        private readonly InventarioContext _context;
        private readonly IAesEncryptionService _encripta;
        public UsuarioService(InventarioContext context, IAesEncryptionService encripta)
        {
            _context = context;
            _encripta = encripta;
        }

        public async Task<UsuarioDTO> Loggin(string usuario , string contrasena )
        {
            var usuarioEncontrado = await _context.Usuarios
                                                   .Where(u =>
                                                       u.Usuario1 == usuario &&
                                                       u.PasswordHash == _encripta.Encrypt(contrasena) && 
                                                       u.Activo == true
                                                     ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
                return null;

            return new UsuarioDTO
            {
                Usuario = usuarioEncontrado.Usuario1,
                Nombre = usuarioEncontrado.Nombre,
                Activo = usuarioEncontrado.Activo,

            };
        }

        public async Task<(int id, string mensaje)> CrearUsuario(UsuarioDTO usuario)
        {
            try
            {
                string validarionErrores = ValidarUsuarioRequest(usuario);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarUsuarioExistente(usuario.Usuario);

                if (existeNombre.Length > 0 )
                {
                    return (0, string.Join(", ", new[] { existeNombre }.Where(e => e.Length > 0)));
                }
                Usuario userDB = new Usuario
                {
                    Nombre = usuario.Nombre,
                    Usuario1 = usuario.Usuario,
                    PasswordHash = _encripta.Encrypt(usuario.PasswordHash),
                    Activo = usuario.Activo,
                    FechaCreacion = DateTime.Now
                };

                var registrarUsuario = await _context.Usuarios.AddAsync (userDB);
                await _context.SaveChangesAsync();  

                int idGenerado = userDB.Id; 

                return (idGenerado, "Usuario creado correctamente");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CrearUsuario: {ex}");
                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }


        public static string ValidarUsuarioRequest(UsuarioDTO usuarioRequest, bool editar = false, bool actualizarConstrasena = true)
        {
            var errores = new List<string>();
            if (editar && usuarioRequest.Id <= 0)
            {
                errores.Add("El campo Id es requerido para la edición");
            }
            if (usuarioRequest == null)
            {
                return "El objeto UsuarioRequest no puede ser nulo";
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
            var parameter = new { Username, UsuarioIDExcluir };
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

                throw;
            }

        }

    }
}
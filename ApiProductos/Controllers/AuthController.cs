using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ApiProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Application.Contrato.IUsuarioService _usuarioService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(Application.Contrato.IUsuarioService usuarioService, ILogger<AuthController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] Models.UsuarioDTO usuario)
        {
            _logger.LogInformation("Petición POST recibida en api/auth/crear para usuario: {Usuario}", usuario.Usuario);
            
            var (id, mensaje) = await _usuarioService.CrearUsuario(usuario);
            if (id == 0)
            {
                _logger.LogWarning("No se pudo crear el usuario {Usuario}. Razón: {Mensaje}", usuario.Usuario, mensaje);
                return BadRequest(new { mensaje });
            }

            _logger.LogInformation("Usuario {Usuario} creado exitosamente con ID: {UserId}", usuario.Usuario, id);
            return Ok(new { id, mensaje });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Loggin([FromBody] Models.LogginDTO usuario)
        {
            _logger.LogInformation("Petición POST recibida en api/auth/login para usuario: {Usuario}", usuario.Usuario);
            
            var usuarioEncontrado = await _usuarioService.Loggin(usuario.Usuario, usuario.Contrasena);
            if (usuarioEncontrado == null)
            {
                _logger.LogWarning("Intento de login denegado para usuario: {Usuario}", usuario.Usuario);
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
            }

            _logger.LogInformation("Login exitoso para usuario: {Usuario}", usuario.Usuario);
            return Ok(usuarioEncontrado);
        }
    }
}
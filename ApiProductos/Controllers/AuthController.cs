using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Application.Contrato.IUsuarioService _usuarioService;

        public AuthController(Application.Contrato.IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] Models.UsuarioDTO usuario)
        {
            var (id, mensaje) = await _usuarioService.CrearUsuario(usuario);
            if (id == 0)
            {
                return BadRequest(new { mensaje });
            }
            return Ok(new { id, mensaje });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Loggin([FromBody] Models.LogginDTO usuario)
        {
            var usuarioEncontrado = await _usuarioService.Loggin(usuario.Usuario, usuario.Contrasena);
            if (usuarioEncontrado == null)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
            }
            return Ok(usuarioEncontrado);
        }


    }
}
using ApiProductos.Filters;
using Application.Contrato;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using System.Threading.Tasks;

namespace ApiProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JwtAuthorization]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        private readonly ILogger<ProveedorController> _logger;

        public ProveedorController(IProveedorService proveedorService, ILogger<ProveedorController> logger)
        {
            _proveedorService = proveedorService;
            _logger = logger;
        }

        [HttpGet]
        [Route("listar")]
        public IActionResult ListarProveedores()
        {
            _logger.LogInformation("Petición GET recibida en api/proveedor/listar.");
            var proveedores = _proveedorService.GetAll();
            return Ok(proveedores);
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> Crear([FromBody] ProveedorDTO proveedor)
        {
            _logger.LogInformation("Petición POST recibida en api/proveedor/crear para proveedor: {Nombre}", proveedor.Nombre);
            var (id, mensaje) = await _proveedorService.CrearProveedor(proveedor);
            if (id > 0)
            {
                _logger.LogInformation("Proveedor {Nombre} creado exitosamente con ID: {Id}.", proveedor.Nombre, id);
                return Ok(new { mensaje = "Proveedor creado exitosamente", id });
            }
            _logger.LogWarning("Fallo al crear proveedor con nombre {Nombre}. Razón: {Mensaje}", proveedor.Nombre, mensaje);
            return BadRequest(new { mensaje });
        }

        [HttpGet]
        [Route("listar/{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation("Petición GET recibida en api/proveedor/listar/{Id}.", id);
            var proveedor = _proveedorService.GetById(id);
            if (proveedor == null)
            {
                _logger.LogWarning("GET api/proveedor/listar/{Id} - Proveedor no encontrado.", id);
                return NotFound(new { mensaje = "Proveedor no encontrado" });
            }
            return Ok(proveedor);
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProveedorDTO proveedor)
        {
            _logger.LogInformation("Petición PUT recibida en api/proveedor/actualizar/{Id}.", id);
            var (exito, mensaje) = await _proveedorService.ActualizarProveedor(id, proveedor);
            if (!exito)
            {
                _logger.LogWarning("Fallo al actualizar proveedor {Id}. Razón: {Mensaje}", id, mensaje);
                return BadRequest(new { mensaje });
            }
            _logger.LogInformation("Proveedor {Id} actualizado exitosamente.", id);
            return Ok(new { mensaje });
        }
    }
}

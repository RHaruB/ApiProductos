using ApiProductos.Filters;
using Application.Contrato;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

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
        public IActionResult Crear([FromBody] ProveedorDTO proveedor)
        {
            _logger.LogInformation("Petición POST recibida en api/proveedor/crear para proveedor: {Nombre}", proveedor.Nombre);
            var resultado = _proveedorService.CrearProveedor(proveedor);
            if (resultado != null)
            {
                _logger.LogInformation("Proveedor {Nombre} creado exitosamente.", proveedor.Nombre);
                return Ok(new { mensaje = "Proveedor creado exitosamente", id = proveedor.Id });
            }
            _logger.LogWarning("Fallo al crear proveedor con nombre {Nombre}.", proveedor.Nombre);
            return BadRequest(new { mensaje = "Error al crear el proveedor" });
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
    }
}

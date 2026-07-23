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
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(IProductoService productoService, ILogger<ProductoController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [HttpGet]
        [Route("listar")]
        public IActionResult ListarProductos()
        {
            _logger.LogInformation("Petición GET recibida en api/producto/listar.");
            var productos = _productoService.GetAll();
            return Ok(productos);
        }

        [HttpGet]
        [Route("listar/{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation("Petición GET recibida en api/producto/listar/{Id}.", id);
            var producto = _productoService.GetById(id);
            if (producto == null)
            {
                _logger.LogWarning("GET api/producto/listar/{Id} - Producto no encontrado.", id);
                return NotFound(new { mensaje = "Producto no encontrado" });
            }
            return Ok(producto);
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> Crear([FromBody] ProductoDTO producto)
        {
            _logger.LogInformation("Petición POST recibida en api/producto/crear para producto con código: {Codigo}", producto.Codigo);
            var (id, mensaje) = await _productoService.CrearProducto(producto);
            if (id > 0)
            {
                _logger.LogInformation("Producto con código {Codigo} creado exitosamente con ID: {Id}.", producto.Codigo, id);
                return Ok(new { mensaje = "Producto creado exitosamente", id });
            }
            _logger.LogWarning("Fallo al crear producto con código {Codigo}. Razón: {Mensaje}", producto.Codigo, mensaje);
            return BadRequest(new { mensaje });
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoDTO producto)
        {
            _logger.LogInformation("Petición PUT recibida en api/producto/actualizar/{Id}.", id);
            var (exito, mensaje) = await _productoService.ActualizarProducto(id, producto);
            if (!exito)
            {
                _logger.LogWarning("Fallo al actualizar producto {Id}. Razón: {Mensaje}", id, mensaje);
                return BadRequest(new { mensaje });
            }
            _logger.LogInformation("Producto {Id} actualizado exitosamente.", id);
            return Ok(new { mensaje });
        }
    }
}
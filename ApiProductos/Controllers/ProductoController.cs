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
        public IActionResult Crear([FromBody] ProductoDTO producto)
        {
            _logger.LogInformation("Petición POST recibida en api/producto/crear para producto con código: {Codigo}", producto.Codigo);
            var resultado = _productoService.CrearProducto(producto);
            if (resultado != null)
            {
                _logger.LogInformation("Producto con código {Codigo} creado exitosamente.", producto.Codigo);
                return Ok(new { mensaje = "Producto creado exitosamente", id = producto.Id });
            }
            _logger.LogWarning("Fallo al crear producto con código {Codigo}.", producto.Codigo);
            return BadRequest(new { mensaje = "Error al crear el producto" });
        }
    }
}
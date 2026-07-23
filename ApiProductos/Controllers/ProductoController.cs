using ApiProductos.Filters;
using Application.Contrato;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ApiProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JwtAuthorization]
    public class ProductoController : ControllerBase
    {
        private readonly Application.Contrato.IProductoService _productoService;
        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        [Route("listar")]
        
        public IActionResult ListarProductos()
        {
            var productos = _productoService.GetAll();
            return Ok(productos);
        }
        [HttpGet]
        [Route("listar/{id}")]
        public IActionResult Get(int id)
        {
            var producto = _productoService.GetById(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = "Producto no encontrado" });
            }
            return Ok(producto);
        }
        [HttpPost]
        [Route("crear")]
        public IActionResult Crear([FromBody] ProductoDTO producto)
        {
            var resultado = _productoService.CrearProducto(producto);
            if (resultado!= null)
            {
                return Ok(new { mensaje = "Producto creado exitosamente", id = producto.Id });
            }
            return BadRequest(new { mensaje = "Error al crear el producto" });
        }

    }
}
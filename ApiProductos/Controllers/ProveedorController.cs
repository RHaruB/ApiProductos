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
    public class ProveedorController : ControllerBase
    {
        private readonly Application.Contrato.IProveedorService _proveedorService;
        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }
        [HttpGet]
        [Route("listar")]
        public IActionResult ListarProveedores()
        {
            var proveedores = _proveedorService.GetAll();
            return Ok(proveedores);
        }
        [HttpPost]
        [Route("crear")]
        public IActionResult Crear([FromBody] ProveedorDTO proveedor)
        {
            var resultado = _proveedorService.CrearProveedor(proveedor);
            if (resultado != null)
            {
                return Ok(new { mensaje = "Proveedor creado exitosamente", id = proveedor.Id });
            }
            return BadRequest(new { mensaje = "Error al crear el proveedor" });
        }
        [HttpGet]
        [Route("listar/{id}")]
        public IActionResult Get(int id)
        {
            var proveedor = _proveedorService.GetById(id);
            if (proveedor == null)
            {
                return NotFound(new { mensaje = "Proveedor no encontrado" });
            }
            return Ok(proveedor);
        }

    }

}

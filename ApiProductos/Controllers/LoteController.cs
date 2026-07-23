using ApiProductos.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace ApiProductos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [JwtAuthorization]

    public class LoteController : ControllerBase
    {
        private readonly Application.Contrato.ILoteService _loteService;

        public LoteController(Application.Contrato.ILoteService loteService)
        {
            _loteService = loteService;
        }

        [HttpGet]
        [Route("listar")]
        public IActionResult ListarLotes()
        {
            var lotes = _loteService.GetAll();
            return Ok(lotes);
        }
        [HttpGet]
        [Route("listar/{id}")]
        public IActionResult Get(int id)
        {
            var lote = _loteService.GetById(id);
            if (lote == null)
            {
                return NotFound(new { mensaje = "Lote no encontrado" });
            }
            return Ok(lote);
        }
        [HttpPost]
                [Route("crear")]
        public async Task<IActionResult> CrearLote([FromBody] LoteDTO lote)
        {
            var resultado = await _loteService.CrearLote(lote);
            if (resultado.id == 0)
            {
                return BadRequest(new { mensaje = resultado.mensaje });
            }
            return Ok(new { id = resultado.id, mensaje = resultado.mensaje });
        }
    }
}
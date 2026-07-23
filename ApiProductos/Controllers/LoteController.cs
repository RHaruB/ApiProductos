using ApiProductos.Filters;
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
    public class LoteController : ControllerBase
    {
        private readonly Application.Contrato.ILoteService _loteService;
        private readonly ILogger<LoteController> _logger;

        public LoteController(Application.Contrato.ILoteService loteService, ILogger<LoteController> logger)
        {
            _loteService = loteService;
            _logger = logger;
        }

        [HttpGet]
        [Route("listar")]
        public IActionResult ListarLotes()
        {
            _logger.LogInformation("Petición GET recibida en api/lote/listar.");
            var lotes = _loteService.GetAll();
            return Ok(lotes);
        }

        [HttpGet]
        [Route("listar/{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation("Petición GET recibida en api/lote/listar/{Id}.", id);
            var lote = _loteService.GetById(id);
            if (lote == null)
            {
                _logger.LogWarning("GET api/lote/listar/{Id} - Lote no encontrado.", id);
                return NotFound(new { mensaje = "Lote no encontrado" });
            }
            return Ok(lote);
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> CrearLote([FromBody] LoteDTO lote)
        {
            _logger.LogInformation("Petición POST recibida en api/lote/crear para lote número: {NumeroLote}, ProductoId: {ProductoId}", lote.NumeroLote, lote.ProductoId);
            var resultado = await _loteService.CrearLote(lote);
            if (resultado.id == 0)
            {
                _logger.LogWarning("Fallo al crear lote. Razón: {Mensaje}", resultado.mensaje);
                return BadRequest(new { mensaje = resultado.mensaje });
            }
            _logger.LogInformation("Lote número {NumeroLote} creado exitosamente con ID {LoteId}.", lote.NumeroLote, resultado.id);
            return Ok(new { id = resultado.id, mensaje = resultado.mensaje });
        }
    }
}
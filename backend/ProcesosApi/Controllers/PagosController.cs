using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _service;

    public PagosController(IPagoService service) => _service = service;

    [HttpGet("procesos/{procesoId}/pagos")]
    public async Task<ActionResult<List<PagoDto>>> ListarPorProceso(string procesoId)
    {
        var result = await _service.ListarPorProcesoAsync(procesoId);
        return Ok(result);
    }

    [HttpPost("procesos/{procesoId}/pagos")]
    public async Task<ActionResult<PagoDto>> Crear(string procesoId, [FromBody] PagoCreateDto dto)
    {
        var result = await _service.CrearAsync(procesoId, dto);
        if (result == null) return NotFound("Proceso no encontrado");
        return CreatedAtAction(nameof(ListarPorProceso), new { procesoId }, result);
    }

    [HttpPut("procesos/{procesoId}/pagos/{pagoId}")]
    public async Task<ActionResult<PagoDto>> Actualizar(string procesoId, string pagoId, [FromBody] PagoCreateDto dto)
    {
        var result = await _service.ActualizarAsync(procesoId, pagoId, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("procesos/{procesoId}/pagos/{pagoId}")]
    public async Task<IActionResult> Eliminar(string procesoId, string pagoId)
    {
        var result = await _service.EliminarAsync(procesoId, pagoId);
        if (!result) return NotFound();
        return NoContent();
    }
}

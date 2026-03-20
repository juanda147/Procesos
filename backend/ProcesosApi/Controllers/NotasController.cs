using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api")]
public class NotasController : ControllerBase
{
    private readonly INotaService _service;

    public NotasController(INotaService service) => _service = service;

    [HttpGet("procesos/{procesoId}/notas")]
    public async Task<ActionResult<List<NotaDto>>> ListarPorProceso(string procesoId)
    {
        var result = await _service.ListarPorProcesoAsync(procesoId);
        return Ok(result);
    }

    [HttpPost("procesos/{procesoId}/notas")]
    public async Task<ActionResult<NotaDto>> Crear(string procesoId, [FromBody] NotaCreateDto dto)
    {
        var result = await _service.CrearAsync(procesoId, dto);
        if (result == null) return NotFound("Proceso no encontrado");
        return CreatedAtAction(nameof(ListarPorProceso), new { procesoId }, result);
    }

    [HttpPut("procesos/{procesoId}/notas/{notaId}")]
    public async Task<ActionResult<NotaDto>> Actualizar(string procesoId, string notaId, [FromBody] NotaCreateDto dto)
    {
        var result = await _service.ActualizarAsync(procesoId, notaId, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("procesos/{procesoId}/notas/{notaId}")]
    public async Task<IActionResult> Eliminar(string procesoId, string notaId)
    {
        var result = await _service.EliminarAsync(procesoId, notaId);
        if (!result) return NotFound();
        return NoContent();
    }
}

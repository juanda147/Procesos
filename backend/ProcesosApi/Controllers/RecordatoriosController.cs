using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api")]
public class RecordatoriosController : ControllerBase
{
    private readonly IRecordatorioService _service;

    public RecordatoriosController(IRecordatorioService service) => _service = service;

    [HttpGet("recordatorios")]
    public async Task<ActionResult<List<RecordatorioDto>>> ListarTodos([FromQuery] string? filtro)
    {
        var result = await _service.ListarTodosAsync(filtro);
        return Ok(result);
    }

    [HttpGet("procesos/{procesoId}/recordatorios")]
    public async Task<ActionResult<List<RecordatorioDto>>> ListarPorProceso(string procesoId)
    {
        var result = await _service.ListarPorProcesoAsync(procesoId);
        return Ok(result);
    }

    [HttpPost("procesos/{procesoId}/recordatorios")]
    public async Task<ActionResult<RecordatorioDto>> Crear(string procesoId, [FromBody] RecordatorioCreateDto dto)
    {
        var result = await _service.CrearAsync(procesoId, dto);
        if (result == null) return NotFound("Proceso no encontrado");
        return CreatedAtAction(nameof(ListarPorProceso), new { procesoId }, result);
    }

    [HttpPut("procesos/{procesoId}/recordatorios/{recordatorioId}")]
    public async Task<ActionResult<RecordatorioDto>> Actualizar(string procesoId, string recordatorioId, [FromBody] RecordatorioCreateDto dto)
    {
        var result = await _service.ActualizarAsync(procesoId, recordatorioId, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("procesos/{procesoId}/recordatorios/{recordatorioId}/completar")]
    public async Task<IActionResult> Completar(string procesoId, string recordatorioId)
    {
        var result = await _service.CompletarAsync(procesoId, recordatorioId);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("procesos/{procesoId}/recordatorios/{recordatorioId}")]
    public async Task<IActionResult> Eliminar(string procesoId, string recordatorioId)
    {
        var result = await _service.EliminarAsync(procesoId, recordatorioId);
        if (!result) return NotFound();
        return NoContent();
    }
}

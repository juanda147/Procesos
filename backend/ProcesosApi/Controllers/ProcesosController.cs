using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcesosController : ControllerBase
{
    private readonly IProcesoService _service;

    public ProcesosController(IProcesoService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PaginatedResultDto<ProcesoListItemDto>>> Listar(
        [FromQuery] string? busqueda,
        [FromQuery] string? ciudad,
        [FromQuery] string? claseProceso,
        [FromQuery] string? estado,
        [FromQuery] string? ingresadoPor,
        [FromQuery] string? ordenarPor,
        [FromQuery] bool ordenDescendente = true,
        [FromQuery] int pagina = 1,
        [FromQuery] int porPagina = 20)
    {
        var result = await _service.ListarAsync(busqueda, ciudad, claseProceso, estado, ingresadoPor, ordenarPor, ordenDescendente, pagina, porPagina);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProcesoDetalleDto>> ObtenerPorId(string id)
    {
        var result = await _service.ObtenerPorIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProcesoDetalleDto>> Crear([FromBody] ProcesoCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProcesoDetalleDto>> Actualizar(string id, [FromBody] ProcesoUpdateDto dto)
    {
        var result = await _service.ActualizarAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(string id)
    {
        var result = await _service.EliminarAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoService _service;

    public CatalogosController(ICatalogoService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<CatalogoDto>>> Listar([FromQuery] string tipo)
    {
        var result = await _service.ListarPorTipoAsync(tipo);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CatalogoDto>> Crear([FromBody] CatalogoCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(Listar), new { tipo = result.Tipo }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CatalogoDto>> Actualizar(string id, [FromBody] CatalogoUpdateDto dto)
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

using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api/campos-globales")]
public class CamposGlobalesController : ControllerBase
{
    private readonly ICampoGlobalService _service;

    public CamposGlobalesController(ICampoGlobalService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<CampoGlobalDto>>> Listar()
    {
        var result = await _service.ListarAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CampoGlobalDto>> Crear([FromBody] CampoGlobalCreateDto dto)
    {
        var result = await _service.CrearAsync(dto);
        return CreatedAtAction(nameof(Listar), null, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CampoGlobalDto>> Actualizar(string id, [FromBody] CampoGlobalUpdateDto dto)
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

using Microsoft.AspNetCore.Mvc;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api")]
public class ImportExportController : ControllerBase
{
    private readonly IExcelService _service;

    public ImportExportController(IExcelService service) => _service = service;

    [HttpPost("importar")]
    public async Task<IActionResult> Importar(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("No se proporcionó un archivo.");

        using var stream = archivo.OpenReadStream();
        var (importados, errores) = await _service.ImportarAsync(stream);

        return Ok(new { importados, errores });
    }

    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar([FromQuery] string? ids)
    {
        List<string>? idList = null;
        if (!string.IsNullOrWhiteSpace(ids))
        {
            idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        var bytes = await _service.ExportarAsync(idList);

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Procesos_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}

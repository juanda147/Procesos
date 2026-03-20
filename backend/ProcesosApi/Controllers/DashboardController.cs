using Microsoft.AspNetCore.Mvc;
using ProcesosApi.DTOs;
using ProcesosApi.Services;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> ObtenerEstadisticas()
    {
        var result = await _service.ObtenerEstadisticasAsync();
        return Ok(result);
    }

    [HttpGet("filtros")]
    public async Task<ActionResult<FiltrosDisponiblesDto>> ObtenerFiltros()
    {
        var result = await _service.ObtenerFiltrosAsync();
        return Ok(result);
    }
}

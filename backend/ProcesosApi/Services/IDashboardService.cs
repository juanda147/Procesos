using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface IDashboardService
{
    Task<DashboardDto> ObtenerEstadisticasAsync();
    Task<FiltrosDisponiblesDto> ObtenerFiltrosAsync();
}

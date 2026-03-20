using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface IProcesoService
{
    Task<PaginatedResultDto<ProcesoListItemDto>> ListarAsync(
        string? busqueda, string? ciudad, string? claseProceso,
        string? estado, string? ingresadoPor,
        string? ordenarPor, bool ordenDescendente,
        int pagina, int porPagina);
    Task<ProcesoDetalleDto?> ObtenerPorIdAsync(string id);
    Task<ProcesoDetalleDto> CrearAsync(ProcesoCreateDto dto);
    Task<ProcesoDetalleDto?> ActualizarAsync(string id, ProcesoUpdateDto dto);
    Task<bool> EliminarAsync(string id);
}

using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface IPagoService
{
    Task<List<PagoDto>> ListarPorProcesoAsync(string procesoId);
    Task<PagoDto?> CrearAsync(string procesoId, PagoCreateDto dto);
    Task<PagoDto?> ActualizarAsync(string procesoId, string pagoId, PagoCreateDto dto);
    Task<bool> EliminarAsync(string procesoId, string pagoId);
}

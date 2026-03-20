using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface IRecordatorioService
{
    Task<List<RecordatorioDto>> ListarTodosAsync(string? filtro);
    Task<List<RecordatorioDto>> ListarPorProcesoAsync(string procesoId);
    Task<RecordatorioDto?> CrearAsync(string procesoId, RecordatorioCreateDto dto);
    Task<RecordatorioDto?> ActualizarAsync(string procesoId, string recordatorioId, RecordatorioCreateDto dto);
    Task<bool> CompletarAsync(string procesoId, string recordatorioId);
    Task<bool> EliminarAsync(string procesoId, string recordatorioId);
}

using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface INotaService
{
    Task<List<NotaDto>> ListarPorProcesoAsync(string procesoId);
    Task<NotaDto?> CrearAsync(string procesoId, NotaCreateDto dto);
    Task<NotaDto?> ActualizarAsync(string procesoId, string notaId, NotaCreateDto dto);
    Task<bool> EliminarAsync(string procesoId, string notaId);
}

using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface ICampoGlobalService
{
    Task<List<CampoGlobalDto>> ListarAsync();
    Task<CampoGlobalDto> CrearAsync(CampoGlobalCreateDto dto);
    Task<CampoGlobalDto?> ActualizarAsync(string id, CampoGlobalUpdateDto dto);
    Task<bool> EliminarAsync(string id);
}

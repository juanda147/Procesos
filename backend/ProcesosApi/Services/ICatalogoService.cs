using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public interface ICatalogoService
{
    Task<List<CatalogoDto>> ListarPorTipoAsync(string tipo);
    Task<CatalogoDto> CrearAsync(CatalogoCreateDto dto);
    Task<CatalogoDto?> ActualizarAsync(string id, CatalogoUpdateDto dto);
    Task<bool> EliminarAsync(string id);
}

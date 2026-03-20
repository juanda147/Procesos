namespace ProcesosApi.Services;

public interface IExcelService
{
    Task<(int importados, List<string> errores)> ImportarAsync(Stream fileStream);
    Task<byte[]> ExportarAsync(List<string>? ids);
}

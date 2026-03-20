namespace ProcesosApi.Services;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml);
    bool EstaConfigurado { get; }
    string? CorreoPorDefecto { get; }
}

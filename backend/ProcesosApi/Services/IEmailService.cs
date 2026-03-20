namespace ProcesosApi.Services;

public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml);
    Task EnviarConAdjuntoAsync(string destinatario, string asunto, string cuerpoHtml,
                                Stream adjunto, string nombreAdjunto);
    bool EstaConfigurado { get; }
    string? CorreoPorDefecto { get; }
}

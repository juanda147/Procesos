using System.Net;
using System.Net.Mail;

namespace ProcesosApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public bool EstaConfigurado
    {
        get
        {
            var usuario = _config["Email:Usuario"];
            var contrasena = _config["Email:Contrasena"];
            return !string.IsNullOrWhiteSpace(usuario) && !string.IsNullOrWhiteSpace(contrasena);
        }
    }

    public string? CorreoPorDefecto => _config["Email:CorreoPorDefecto"];

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        await EnviarInternoAsync(destinatario, asunto, cuerpoHtml, null, null);
    }

    public async Task EnviarConAdjuntoAsync(string destinatario, string asunto, string cuerpoHtml,
                                             Stream adjunto, string nombreAdjunto)
    {
        await EnviarInternoAsync(destinatario, asunto, cuerpoHtml, adjunto, nombreAdjunto);
    }

    private async Task EnviarInternoAsync(string destinatario, string asunto, string cuerpoHtml,
                                           Stream? adjunto, string? nombreAdjunto)
    {
        if (!EstaConfigurado)
        {
            _logger.LogWarning("Email no configurado. No se puede enviar correo a {Destinatario}", destinatario);
            return;
        }

        var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
        var usuario = _config["Email:Usuario"]!;
        var contrasena = _config["Email:Contrasena"]!;
        var nombreRemitente = _config["Email:NombreRemitente"] ?? "Procesos - Sistema Legal";

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(usuario, contrasena),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(usuario, nombreRemitente),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };
            message.To.Add(destinatario);

            if (adjunto != null && !string.IsNullOrWhiteSpace(nombreAdjunto))
            {
                adjunto.Position = 0;
                message.Attachments.Add(new Attachment(adjunto, nombreAdjunto, "application/zip"));
            }

            await client.SendMailAsync(message);
            _logger.LogInformation("Email enviado a {Destinatario}: {Asunto}", destinatario, asunto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando email a {Destinatario}: {Asunto}", destinatario, asunto);
        }
    }
}

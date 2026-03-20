using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class RecordatorioNotificacionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecordatorioNotificacionService> _logger;

    public RecordatorioNotificacionService(IServiceProvider serviceProvider, ILogger<RecordatorioNotificacionService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio de notificaciones de recordatorios iniciado");

        // Esperar 10 segundos antes del primer chequeo para que la app termine de iniciar
        try { await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); }
        catch (OperationCanceledException) { return; }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await VerificarYEnviarNotificaciones();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el ciclo de notificaciones de recordatorios");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Servicio de notificaciones de recordatorios detenido");
    }

    private async Task VerificarYEnviarNotificaciones()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        if (!emailService.EstaConfigurado)
        {
            _logger.LogDebug("Email no configurado, saltando verificacion de notificaciones");
            return;
        }

        var today = DateTime.UtcNow.Date;
        var procesos = await db.Procesos
            .Find(p => p.Activo && p.Recordatorios.Any(r =>
                !r.Completado && !r.NotificacionEnviada &&
                r.FechaVencimiento >= today && r.FechaVencimiento < today.AddDays(1)))
            .ToListAsync();

        foreach (var proceso in procesos)
        {
            var recordatoriosHoy = proceso.Recordatorios
                .Where(r => !r.Completado && !r.NotificacionEnviada &&
                            r.FechaVencimiento.Date == today)
                .ToList();

            foreach (var rec in recordatoriosHoy)
            {
                var destinatario = rec.CorreoNotificacion ?? emailService.CorreoPorDefecto;
                if (string.IsNullOrWhiteSpace(destinatario)) continue;

                var asunto = $"⚠️ Recordatorio vence hoy: {rec.Titulo}";
                var cuerpo = GenerarEmailVencimiento(rec, proceso);

                await emailService.EnviarAsync(destinatario, asunto, cuerpo);

                // Marcar como enviada
                var filter = Builders<Proceso>.Filter.Eq(p => p.Id, proceso.Id) &
                             Builders<Proceso>.Filter.ElemMatch(p => p.Recordatorios, r => r.Id == rec.Id);
                var update = Builders<Proceso>.Update.Set("Recordatorios.$.NotificacionEnviada", true);
                await db.Procesos.UpdateOneAsync(filter, update);

                _logger.LogInformation("Notificacion de vencimiento enviada para recordatorio {RecId} del proceso {ProcesoId}",
                    rec.Id, proceso.Id);
            }
        }
    }

    private static string GenerarEmailVencimiento(Recordatorio rec, Proceso proceso)
    {
        var descripcion = string.IsNullOrWhiteSpace(rec.Descripcion)
            ? ""
            : $"<p><strong>Descripción:</strong> {rec.Descripcion}</p>";

        return $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
            <div style='background-color: #ff9800; color: white; padding: 15px 20px; border-radius: 8px 8px 0 0;'>
                <h2 style='margin: 0;'>⚠️ Recordatorio Vence Hoy</h2>
            </div>
            <div style='border: 1px solid #e0e0e0; border-top: none; padding: 20px; border-radius: 0 0 8px 8px;'>
                <h3 style='color: #1565c0; margin-top: 0;'>{rec.Titulo}</h3>
                {descripcion}
                <p><strong>Fecha de Vencimiento:</strong> {rec.FechaVencimiento:dd/MM/yyyy}</p>
                <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 15px 0;'/>
                <p style='color: #666; font-size: 14px;'>
                    <strong>Proceso:</strong> {proceso.Demandante} vs. {proceso.Demandado}<br/>
                    <strong>Radicado:</strong> {proceso.Radicado}
                </p>
                <p style='color: #999; font-size: 12px; margin-top: 20px;'>
                    Este correo fue enviado automáticamente por Procesos - Sistema de Gestión Legal
                </p>
            </div>
        </div>";
    }
}

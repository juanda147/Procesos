using System.IO.Compression;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ProcesosApi.Data;

namespace ProcesosApi.Services;

public class BackupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackupService> _logger;
    private readonly IConfiguration _config;

    public BackupService(IServiceProvider serviceProvider, ILogger<BackupService> logger, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Esperar 30 segundos para que la app arranque
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        var intervaloDias = int.Parse(_config["Backup:IntervaloDias"] ?? "30");
        if (intervaloDias <= 0) intervaloDias = 30;

        _logger.LogInformation("BackupService iniciado. Intervalo: {Dias} días", intervaloDias);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EjecutarBackupAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando backup de MongoDB");
            }

            try
            {
                await Task.Delay(TimeSpan.FromDays(intervaloDias), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("BackupService detenido");
    }

    private async Task EjecutarBackupAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var correoDestino = _config["Backup:CorreoDestino"];
        if (string.IsNullOrWhiteSpace(correoDestino))
            correoDestino = emailService.CorreoPorDefecto;

        if (string.IsNullOrWhiteSpace(correoDestino))
        {
            _logger.LogWarning("Backup: No hay correo destino configurado. Saltando backup.");
            return;
        }

        if (!emailService.EstaConfigurado)
        {
            _logger.LogWarning("Backup: Email no configurado. Saltando backup.");
            return;
        }

        _logger.LogInformation("Iniciando backup de MongoDB...");

        // Exportar colecciones a JSON
        var procesosJson = await ExportarColeccionAsync(dbContext.Procesos, ct);
        var catalogosJson = await ExportarColeccionAsync(dbContext.Catalogos, ct);
        var camposJson = await ExportarColeccionAsync(dbContext.CamposGlobales, ct);

        var countProcesos = procesosJson.count;
        var countCatalogos = catalogosJson.count;
        var countCampos = camposJson.count;

        // Crear ZIP en memoria
        using var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            AgregarEntradaZip(archive, "procesos.json", procesosJson.json);
            AgregarEntradaZip(archive, "catalogos.json", catalogosJson.json);
            AgregarEntradaZip(archive, "campos_globales.json", camposJson.json);
        }

        zipStream.Position = 0;
        var tamanoMB = Math.Round(zipStream.Length / 1024.0 / 1024.0, 2);
        var fecha = DateTime.Now.ToString("yyyy-MM-dd");

        _logger.LogInformation(
            "Backup generado: {Procesos} procesos, {Catalogos} catálogos, {Campos} campos globales. Tamaño ZIP: {Tamano}MB",
            countProcesos, countCatalogos, countCampos, tamanoMB);

        var asunto = $"Backup ProcesosDb — {fecha}";
        var cuerpo = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #4a6741;'>Backup de Base de Datos</h2>
                <p>Se ha generado un backup automático de la base de datos <strong>ProcesosDb</strong>.</p>

                <table style='border-collapse: collapse; width: 100%; margin: 16px 0;'>
                    <tr style='background-color: #f0ebe3;'>
                        <th style='padding: 8px 12px; text-align: left; border: 1px solid #d5cec4;'>Colección</th>
                        <th style='padding: 8px 12px; text-align: right; border: 1px solid #d5cec4;'>Documentos</th>
                    </tr>
                    <tr>
                        <td style='padding: 8px 12px; border: 1px solid #d5cec4;'>Procesos</td>
                        <td style='padding: 8px 12px; text-align: right; border: 1px solid #d5cec4;'>{countProcesos}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 12px; border: 1px solid #d5cec4;'>Catálogos</td>
                        <td style='padding: 8px 12px; text-align: right; border: 1px solid #d5cec4;'>{countCatalogos}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px 12px; border: 1px solid #d5cec4;'>Campos Globales</td>
                        <td style='padding: 8px 12px; text-align: right; border: 1px solid #d5cec4;'>{countCampos}</td>
                    </tr>
                </table>

                <p><strong>Tamaño del archivo:</strong> {tamanoMB} MB</p>
                <p><strong>Fecha:</strong> {DateTime.Now:dddd, dd MMMM yyyy HH:mm}</p>

                <hr style='border: 1px solid #d5cec4; margin: 20px 0;' />
                <p style='color: #636e72; font-size: 12px;'>
                    Para restaurar este backup, descomprima el archivo ZIP e importe los archivos JSON
                    en MongoDB usando mongoimport o la herramienta de su preferencia.
                </p>
            </div>";

        var nombreArchivo = $"ProcesosDb_backup_{fecha}.zip";

        await emailService.EnviarConAdjuntoAsync(correoDestino, asunto, cuerpo, zipStream, nombreArchivo);
        _logger.LogInformation("Backup enviado exitosamente a {Correo}", correoDestino);
    }

    private static async Task<(string json, int count)> ExportarColeccionAsync<T>(
        IMongoCollection<T> collection, CancellationToken ct)
    {
        var documentos = await collection
            .Find(FilterDefinition<T>.Empty)
            .ToListAsync(ct);

        // Serializar usando el driver de MongoDB para preservar tipos BSON (ObjectId, dates, etc.)
        var jsonSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson, Indent = true };
        var jsonDocs = documentos
            .Select(d => d!.ToBsonDocument().ToJson(jsonSettings))
            .ToList();

        var json = "[\n" + string.Join(",\n", jsonDocs) + "\n]";
        return (json, documentos.Count);
    }

    private static void AgregarEntradaZip(ZipArchive archive, string nombre, string contenido)
    {
        var entry = archive.CreateEntry(nombre, CompressionLevel.Optimal);
        using var writer = new StreamWriter(entry.Open());
        writer.Write(contenido);
    }
}

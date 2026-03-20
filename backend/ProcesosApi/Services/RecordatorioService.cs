using MongoDB.Bson;
using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class RecordatorioService : IRecordatorioService
{
    private readonly MongoDbContext _db;
    private readonly IEmailService _emailService;

    public RecordatorioService(MongoDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<List<RecordatorioDto>> ListarTodosAsync(string? filtro)
    {
        var today = DateTime.UtcNow.Date;
        var procesos = await _db.Procesos.Find(p => p.Activo).ToListAsync();

        var recordatorios = procesos
            .SelectMany(p => p.Recordatorios.Select(r => new RecordatorioDto
            {
                Id = r.Id,
                ProcesoId = p.Id,
                Titulo = r.Titulo,
                Descripcion = r.Descripcion,
                FechaVencimiento = r.FechaVencimiento,
                Completado = r.Completado,
                FechaCreacion = r.FechaCreacion,
                CorreoNotificacion = r.CorreoNotificacion,
                DemandanteDelProceso = p.Demandante,
                RadicadoDelProceso = p.Radicado
            }));

        recordatorios = filtro?.ToLower() switch
        {
            "vencidos" => recordatorios.Where(r => !r.Completado && r.FechaVencimiento.Date < today),
            "pendientes" => recordatorios.Where(r => !r.Completado && r.FechaVencimiento.Date >= today),
            "completados" => recordatorios.Where(r => r.Completado),
            _ => recordatorios.Where(r => !r.Completado)
        };

        return recordatorios.OrderBy(r => r.FechaVencimiento).ToList();
    }

    public async Task<List<RecordatorioDto>> ListarPorProcesoAsync(string procesoId)
    {
        var proceso = await _db.Procesos.Find(p => p.Id == procesoId).FirstOrDefaultAsync();
        if (proceso == null) return new();

        return proceso.Recordatorios.OrderBy(r => r.FechaVencimiento).Select(r => new RecordatorioDto
        {
            Id = r.Id,
            ProcesoId = procesoId,
            Titulo = r.Titulo,
            Descripcion = r.Descripcion,
            FechaVencimiento = r.FechaVencimiento,
            Completado = r.Completado,
            FechaCreacion = r.FechaCreacion,
            CorreoNotificacion = r.CorreoNotificacion,
            DemandanteDelProceso = proceso.Demandante,
            RadicadoDelProceso = proceso.Radicado
        }).ToList();
    }

    public async Task<RecordatorioDto?> CrearAsync(string procesoId, RecordatorioCreateDto dto)
    {
        var proceso = await _db.Procesos.Find(p => p.Id == procesoId && p.Activo).FirstOrDefaultAsync();
        if (proceso == null) return null;

        var recordatorio = new Recordatorio
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            FechaVencimiento = dto.FechaVencimiento,
            CorreoNotificacion = dto.CorreoNotificacion,
            FechaCreacion = DateTime.UtcNow
        };

        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId);
        var update = Builders<Proceso>.Update.Push(p => p.Recordatorios, recordatorio);
        await _db.Procesos.UpdateOneAsync(filter, update);

        // Enviar email de confirmación al crear
        var destinatario = recordatorio.CorreoNotificacion ?? _emailService.CorreoPorDefecto;
        if (!string.IsNullOrWhiteSpace(destinatario))
        {
            var descripcionHtml = string.IsNullOrWhiteSpace(recordatorio.Descripcion)
                ? ""
                : $"<p><strong>Descripción:</strong> {recordatorio.Descripcion}</p>";

            var cuerpo = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background-color: #1565c0; color: white; padding: 15px 20px; border-radius: 8px 8px 0 0;'>
                    <h2 style='margin: 0;'>📋 Nuevo Recordatorio Creado</h2>
                </div>
                <div style='border: 1px solid #e0e0e0; border-top: none; padding: 20px; border-radius: 0 0 8px 8px;'>
                    <h3 style='color: #1565c0; margin-top: 0;'>{recordatorio.Titulo}</h3>
                    {descripcionHtml}
                    <p><strong>Fecha de Vencimiento:</strong> {recordatorio.FechaVencimiento:dd/MM/yyyy}</p>
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

            _ = Task.Run(() => _emailService.EnviarAsync(destinatario, $"Nuevo Recordatorio: {recordatorio.Titulo}", cuerpo));
        }

        return new RecordatorioDto
        {
            Id = recordatorio.Id,
            ProcesoId = procesoId,
            Titulo = recordatorio.Titulo,
            Descripcion = recordatorio.Descripcion,
            FechaVencimiento = recordatorio.FechaVencimiento,
            Completado = recordatorio.Completado,
            FechaCreacion = recordatorio.FechaCreacion,
            CorreoNotificacion = recordatorio.CorreoNotificacion,
            DemandanteDelProceso = proceso.Demandante,
            RadicadoDelProceso = proceso.Radicado
        };
    }

    public async Task<RecordatorioDto?> ActualizarAsync(string procesoId, string recordatorioId, RecordatorioCreateDto dto)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId) &
                     Builders<Proceso>.Filter.ElemMatch(p => p.Recordatorios, r => r.Id == recordatorioId);

        var update = Builders<Proceso>.Update
            .Set("Recordatorios.$.Titulo", dto.Titulo)
            .Set("Recordatorios.$.Descripcion", dto.Descripcion)
            .Set("Recordatorios.$.FechaVencimiento", dto.FechaVencimiento)
            .Set("Recordatorios.$.CorreoNotificacion", dto.CorreoNotificacion)
            .Set("Recordatorios.$.NotificacionEnviada", false);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        if (result.ModifiedCount == 0) return null;

        var proceso = await _db.Procesos.Find(p => p.Id == procesoId).FirstOrDefaultAsync();
        var rec = proceso?.Recordatorios.FirstOrDefault(r => r.Id == recordatorioId);
        if (rec == null || proceso == null) return null;

        return new RecordatorioDto
        {
            Id = rec.Id,
            ProcesoId = procesoId,
            Titulo = rec.Titulo,
            Descripcion = rec.Descripcion,
            FechaVencimiento = rec.FechaVencimiento,
            Completado = rec.Completado,
            FechaCreacion = rec.FechaCreacion,
            CorreoNotificacion = rec.CorreoNotificacion,
            DemandanteDelProceso = proceso.Demandante,
            RadicadoDelProceso = proceso.Radicado
        };
    }

    public async Task<bool> CompletarAsync(string procesoId, string recordatorioId)
    {
        var proceso = await _db.Procesos.Find(p => p.Id == procesoId).FirstOrDefaultAsync();
        var rec = proceso?.Recordatorios.FirstOrDefault(r => r.Id == recordatorioId);
        if (rec == null) return false;

        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId) &
                     Builders<Proceso>.Filter.ElemMatch(p => p.Recordatorios, r => r.Id == recordatorioId);
        var update = Builders<Proceso>.Update.Set("Recordatorios.$.Completado", !rec.Completado);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> EliminarAsync(string procesoId, string recordatorioId)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId);
        var update = Builders<Proceso>.Update.PullFilter(p => p.Recordatorios, r => r.Id == recordatorioId);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}

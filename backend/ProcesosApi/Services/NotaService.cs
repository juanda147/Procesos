using MongoDB.Bson;
using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class NotaService : INotaService
{
    private readonly MongoDbContext _db;

    public NotaService(MongoDbContext db) => _db = db;

    public async Task<List<NotaDto>> ListarPorProcesoAsync(string procesoId)
    {
        var proceso = await _db.Procesos.Find(p => p.Id == procesoId).FirstOrDefaultAsync();
        if (proceso == null) return new();

        return proceso.Notas.OrderByDescending(n => n.FechaCreacion).Select(n => new NotaDto
        {
            Id = n.Id,
            ProcesoId = procesoId,
            Contenido = n.Contenido,
            FechaCreacion = n.FechaCreacion,
            FechaActualizacion = n.FechaActualizacion
        }).ToList();
    }

    public async Task<NotaDto?> CrearAsync(string procesoId, NotaCreateDto dto)
    {
        var nota = new Nota
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Contenido = dto.Contenido,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId) &
                     Builders<Proceso>.Filter.Eq(p => p.Activo, true);
        var update = Builders<Proceso>.Update.Push(p => p.Notas, nota);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        if (result.ModifiedCount == 0) return null;

        return new NotaDto
        {
            Id = nota.Id,
            ProcesoId = procesoId,
            Contenido = nota.Contenido,
            FechaCreacion = nota.FechaCreacion,
            FechaActualizacion = nota.FechaActualizacion
        };
    }

    public async Task<NotaDto?> ActualizarAsync(string procesoId, string notaId, NotaCreateDto dto)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId) &
                     Builders<Proceso>.Filter.ElemMatch(p => p.Notas, n => n.Id == notaId);

        var update = Builders<Proceso>.Update
            .Set("Notas.$.Contenido", dto.Contenido)
            .Set("Notas.$.FechaActualizacion", DateTime.UtcNow);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        if (result.ModifiedCount == 0) return null;

        var proceso = await _db.Procesos.Find(p => p.Id == procesoId).FirstOrDefaultAsync();
        var nota = proceso?.Notas.FirstOrDefault(n => n.Id == notaId);
        if (nota == null) return null;

        return new NotaDto
        {
            Id = nota.Id,
            ProcesoId = procesoId,
            Contenido = nota.Contenido,
            FechaCreacion = nota.FechaCreacion,
            FechaActualizacion = nota.FechaActualizacion
        };
    }

    public async Task<bool> EliminarAsync(string procesoId, string notaId)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId);
        var update = Builders<Proceso>.Update.PullFilter(p => p.Notas, n => n.Id == notaId);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}

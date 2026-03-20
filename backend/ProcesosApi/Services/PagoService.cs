using MongoDB.Bson;
using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class PagoService : IPagoService
{
    private readonly MongoDbContext _db;

    public PagoService(MongoDbContext db) => _db = db;

    public async Task<List<PagoDto>> ListarPorProcesoAsync(string procesoId)
    {
        var proceso = await _db.Procesos.Find(p => p.Id == procesoId && p.Activo).FirstOrDefaultAsync();
        if (proceso == null) return new();

        return proceso.Pagos.OrderByDescending(p => p.Fecha).Select(p => new PagoDto
        {
            Id = p.Id,
            ProcesoId = procesoId,
            Fecha = p.Fecha,
            Monto = p.Monto,
            Concepto = p.Concepto,
            MetodoPago = p.MetodoPago,
            FechaCreacion = p.FechaCreacion
        }).ToList();
    }

    public async Task<PagoDto?> CrearAsync(string procesoId, PagoCreateDto dto)
    {
        var pago = new Pago
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Fecha = dto.Fecha,
            Monto = dto.Monto,
            Concepto = dto.Concepto,
            MetodoPago = dto.MetodoPago,
            FechaCreacion = DateTime.UtcNow
        };

        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId) &
                     Builders<Proceso>.Filter.Eq(p => p.Activo, true);
        var update = Builders<Proceso>.Update.Push(p => p.Pagos, pago);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        if (result.ModifiedCount == 0) return null;

        return new PagoDto
        {
            Id = pago.Id,
            ProcesoId = procesoId,
            Fecha = pago.Fecha,
            Monto = pago.Monto,
            Concepto = pago.Concepto,
            MetodoPago = pago.MetodoPago,
            FechaCreacion = pago.FechaCreacion
        };
    }

    public async Task<PagoDto?> ActualizarAsync(string procesoId, string pagoId, PagoCreateDto dto)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId) &
                     Builders<Proceso>.Filter.ElemMatch(p => p.Pagos, pa => pa.Id == pagoId);

        var update = Builders<Proceso>.Update
            .Set("Pagos.$.Fecha", dto.Fecha)
            .Set("Pagos.$.Monto", dto.Monto)
            .Set("Pagos.$.Concepto", dto.Concepto)
            .Set("Pagos.$.MetodoPago", dto.MetodoPago);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        if (result.ModifiedCount == 0) return null;

        var proceso = await _db.Procesos.Find(p => p.Id == procesoId).FirstOrDefaultAsync();
        var pago = proceso?.Pagos.FirstOrDefault(p => p.Id == pagoId);
        if (pago == null) return null;

        return new PagoDto
        {
            Id = pago.Id,
            ProcesoId = procesoId,
            Fecha = pago.Fecha,
            Monto = pago.Monto,
            Concepto = pago.Concepto,
            MetodoPago = pago.MetodoPago,
            FechaCreacion = pago.FechaCreacion
        };
    }

    public async Task<bool> EliminarAsync(string procesoId, string pagoId)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, procesoId);
        var update = Builders<Proceso>.Update.PullFilter(p => p.Pagos, pa => pa.Id == pagoId);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}

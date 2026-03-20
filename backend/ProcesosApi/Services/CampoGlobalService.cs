using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class CampoGlobalService : ICampoGlobalService
{
    private readonly MongoDbContext _db;

    public CampoGlobalService(MongoDbContext db) => _db = db;

    public async Task<List<CampoGlobalDto>> ListarAsync()
    {
        var filter = Builders<CampoGlobal>.Filter.Eq(c => c.Activo, true);
        var campos = await _db.CamposGlobales
            .Find(filter)
            .SortBy(c => c.Orden)
            .ToListAsync();

        return campos.Select(c => new CampoGlobalDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Tipo = c.Tipo,
            Orden = c.Orden
        }).ToList();
    }

    public async Task<CampoGlobalDto> CrearAsync(CampoGlobalCreateDto dto)
    {
        var maxOrden = await _db.CamposGlobales
            .Find(c => c.Activo)
            .SortByDescending(c => c.Orden)
            .Limit(1)
            .FirstOrDefaultAsync();

        var campo = new CampoGlobal
        {
            Nombre = dto.Nombre,
            Tipo = dto.Tipo,
            Orden = (maxOrden?.Orden ?? 0) + 1,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _db.CamposGlobales.InsertOneAsync(campo);

        return new CampoGlobalDto
        {
            Id = campo.Id,
            Nombre = campo.Nombre,
            Tipo = campo.Tipo,
            Orden = campo.Orden
        };
    }

    public async Task<CampoGlobalDto?> ActualizarAsync(string id, CampoGlobalUpdateDto dto)
    {
        var update = Builders<CampoGlobal>.Update
            .Set(c => c.Nombre, dto.Nombre)
            .Set(c => c.Tipo, dto.Tipo);

        var options = new FindOneAndUpdateOptions<CampoGlobal> { ReturnDocument = ReturnDocument.After };
        var campo = await _db.CamposGlobales.FindOneAndUpdateAsync(
            c => c.Id == id && c.Activo, update, options);

        if (campo == null) return null;

        return new CampoGlobalDto
        {
            Id = campo.Id,
            Nombre = campo.Nombre,
            Tipo = campo.Tipo,
            Orden = campo.Orden
        };
    }

    public async Task<bool> EliminarAsync(string id)
    {
        var update = Builders<CampoGlobal>.Update.Set(c => c.Activo, false);
        var result = await _db.CamposGlobales.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }
}

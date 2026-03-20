using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class CatalogoService : ICatalogoService
{
    private readonly MongoDbContext _db;

    public CatalogoService(MongoDbContext db) => _db = db;

    public async Task<List<CatalogoDto>> ListarPorTipoAsync(string tipo)
    {
        var filter = Builders<Catalogo>.Filter.Eq(c => c.Tipo, tipo) &
                     Builders<Catalogo>.Filter.Eq(c => c.Activo, true);

        var catalogos = await _db.Catalogos
            .Find(filter)
            .SortBy(c => c.Orden)
            .ToListAsync();

        return catalogos.Select(c => new CatalogoDto
        {
            Id = c.Id,
            Tipo = c.Tipo,
            Valor = c.Valor,
            Orden = c.Orden
        }).ToList();
    }

    public async Task<CatalogoDto> CrearAsync(CatalogoCreateDto dto)
    {
        // Get max order for this type
        var maxOrden = await _db.Catalogos
            .Find(c => c.Tipo == dto.Tipo && c.Activo)
            .SortByDescending(c => c.Orden)
            .Limit(1)
            .FirstOrDefaultAsync();

        var catalogo = new Catalogo
        {
            Tipo = dto.Tipo,
            Valor = dto.Valor,
            Orden = (maxOrden?.Orden ?? 0) + 1,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        await _db.Catalogos.InsertOneAsync(catalogo);

        return new CatalogoDto
        {
            Id = catalogo.Id,
            Tipo = catalogo.Tipo,
            Valor = catalogo.Valor,
            Orden = catalogo.Orden
        };
    }

    public async Task<CatalogoDto?> ActualizarAsync(string id, CatalogoUpdateDto dto)
    {
        var update = Builders<Catalogo>.Update.Set(c => c.Valor, dto.Valor);
        var options = new FindOneAndUpdateOptions<Catalogo> { ReturnDocument = ReturnDocument.After };
        var catalogo = await _db.Catalogos.FindOneAndUpdateAsync(c => c.Id == id && c.Activo, update, options);

        if (catalogo == null) return null;

        return new CatalogoDto
        {
            Id = catalogo.Id,
            Tipo = catalogo.Tipo,
            Valor = catalogo.Valor,
            Orden = catalogo.Orden
        };
    }

    public async Task<bool> EliminarAsync(string id)
    {
        var update = Builders<Catalogo>.Update.Set(c => c.Activo, false);
        var result = await _db.Catalogos.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }
}

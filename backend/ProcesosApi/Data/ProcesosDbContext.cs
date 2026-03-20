using MongoDB.Driver;
using ProcesosApi.Models;

namespace ProcesosApi.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDb:ConnectionString"];
        var databaseName = configuration["MongoDb:DatabaseName"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Proceso> Procesos => _database.GetCollection<Proceso>("procesos");
    public IMongoCollection<Catalogo> Catalogos => _database.GetCollection<Catalogo>("catalogos");
    public IMongoCollection<CampoGlobal> CamposGlobales => _database.GetCollection<CampoGlobal>("camposGlobales");

    public async Task CreateIndexesAsync()
    {
        await Procesos.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Proceso>(Builders<Proceso>.IndexKeys.Ascending(p => p.Activo)),
            new CreateIndexModel<Proceso>(Builders<Proceso>.IndexKeys.Ascending(p => p.Ciudad)),
            new CreateIndexModel<Proceso>(Builders<Proceso>.IndexKeys.Ascending(p => p.ClaseProceso)),
        });

        await Catalogos.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Catalogo>(
                Builders<Catalogo>.IndexKeys
                    .Ascending(c => c.Tipo)
                    .Ascending(c => c.Activo)
                    .Ascending(c => c.Orden))
        });

        await CamposGlobales.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<CampoGlobal>(
                Builders<CampoGlobal>.IndexKeys
                    .Ascending(c => c.Activo)
                    .Ascending(c => c.Orden))
        });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MongoDB.Bson;
using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.Models;

namespace ProcesosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MigracionController : ControllerBase
{
    private readonly MongoDbContext _db;
    private readonly IWebHostEnvironment _env;

    public MigracionController(MongoDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost("sqlite-a-mongodb")]
    public async Task<IActionResult> MigrarDesdeSquLite([FromQuery] bool forzar = false)
    {
        var sqlitePath = Path.Combine(_env.ContentRootPath, "procesos.db");
        if (!System.IO.File.Exists(sqlitePath))
            return NotFound(new { error = "No se encontro el archivo procesos.db" });

        // Check if MongoDB already has data
        var existingCount = await _db.Procesos.CountDocumentsAsync(FilterDefinition<Proceso>.Empty);
        if (existingCount > 0 && !forzar)
            return BadRequest(new { error = $"MongoDB ya tiene {existingCount} procesos. Use ?forzar=true para limpiar y re-migrar." });

        if (existingCount > 0 && forzar)
        {
            await _db.Procesos.DeleteManyAsync(FilterDefinition<Proceso>.Empty);
            await _db.Catalogos.DeleteManyAsync(FilterDefinition<Catalogo>.Empty);
        }

        var connectionString = $"Data Source={sqlitePath};Mode=ReadOnly";
        using var conn = new SqliteConnection(connectionString);
        conn.Open();

        // Read all procesos from SQLite
        var procesos = new List<Proceso>();
        var procesoIdMap = new Dictionary<long, string>(); // old int ID -> new ObjectId string

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Procesos";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var oldId = reader.GetInt64(reader.GetOrdinal("Id"));
                var newId = ObjectId.GenerateNewId().ToString();
                procesoIdMap[oldId] = newId;

                var proceso = new Proceso
                {
                    Id = newId,
                    Fecha = GetDateTime(reader, "Fecha"),
                    Demandante = GetString(reader, "Demandante"),
                    Demandado = GetString(reader, "Demandado"),
                    Radicado = GetString(reader, "Radicado"),
                    Juzgado = GetString(reader, "Juzgado"),
                    Ciudad = GetString(reader, "Ciudad"),
                    ClaseProceso = GetString(reader, "ClaseProceso"),
                    Representamos = GetNullableString(reader, "Representamos"),
                    ProcesoIngresadoPor = GetNullableString(reader, "ProcesoIngresadoPor"),
                    Honorarios = GetNullableString(reader, "Honorarios"),
                    EstadoActual = GetNullableString(reader, "EstadoActual"),
                    Activo = GetBool(reader, "Activo"),
                    FechaCreacion = GetDateTime(reader, "FechaCreacion"),
                    FechaActualizacion = GetDateTime(reader, "FechaActualizacion"),
                    Comisiones = new List<Comision>(),
                    Pagos = new List<Pago>(),
                    Notas = new List<Nota>(),
                    Recordatorios = new List<Recordatorio>(),
                };

                // Convert PorcentajePaola / PorcentajeMIsabel to Comisiones array
                var porcentajePaola = GetNullableString(reader, "PorcentajePaola");
                var porcentajeMIsabel = GetNullableString(reader, "PorcentajeMIsabel");

                if (!string.IsNullOrWhiteSpace(porcentajePaola))
                    proceso.Comisiones.Add(new Comision { Persona = "Paola", Porcentaje = porcentajePaola });
                if (!string.IsNullOrWhiteSpace(porcentajeMIsabel))
                    proceso.Comisiones.Add(new Comision { Persona = "M. Isabel", Porcentaje = porcentajeMIsabel });

                procesos.Add(proceso);
            }
        }

        // Read Pagos and embed into their Proceso
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Pagos";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var procesoId = reader.GetInt64(reader.GetOrdinal("ProcesoId"));
                if (!procesoIdMap.TryGetValue(procesoId, out var newProcesoId))
                    continue;

                var proceso = procesos.First(p => p.Id == newProcesoId);
                proceso.Pagos.Add(new Pago
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Fecha = GetDateTime(reader, "Fecha"),
                    Monto = GetDecimal(reader, "Monto"),
                    Concepto = GetNullableString(reader, "Concepto"),
                    MetodoPago = GetNullableString(reader, "MetodoPago"),
                    FechaCreacion = GetDateTime(reader, "FechaCreacion"),
                });
            }
        }

        // Read Notas and embed into their Proceso
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Notas";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var procesoId = reader.GetInt64(reader.GetOrdinal("ProcesoId"));
                if (!procesoIdMap.TryGetValue(procesoId, out var newProcesoId))
                    continue;

                var proceso = procesos.First(p => p.Id == newProcesoId);
                proceso.Notas.Add(new Nota
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Contenido = GetString(reader, "Contenido"),
                    FechaCreacion = GetDateTime(reader, "FechaCreacion"),
                    FechaActualizacion = GetDateTime(reader, "FechaActualizacion"),
                });
            }
        }

        // Read Recordatorios and embed into their Proceso
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM Recordatorios";
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var procesoId = reader.GetInt64(reader.GetOrdinal("ProcesoId"));
                if (!procesoIdMap.TryGetValue(procesoId, out var newProcesoId))
                    continue;

                var proceso = procesos.First(p => p.Id == newProcesoId);
                proceso.Recordatorios.Add(new Recordatorio
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Titulo = GetString(reader, "Titulo"),
                    Descripcion = GetNullableString(reader, "Descripcion"),
                    FechaVencimiento = GetDateTime(reader, "FechaVencimiento"),
                    Completado = GetBool(reader, "Completado"),
                    FechaCreacion = GetDateTime(reader, "FechaCreacion"),
                });
            }
        }

        conn.Close();

        // Insert all procesos into MongoDB
        if (procesos.Count > 0)
            await _db.Procesos.InsertManyAsync(procesos);

        // Seed Catalogos from distinct values
        var catalogosToInsert = new List<Catalogo>();
        var now = DateTime.UtcNow;

        var ciudades = procesos.Select(p => p.Ciudad).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c).ToList();
        for (int i = 0; i < ciudades.Count; i++)
            catalogosToInsert.Add(new Catalogo { Tipo = "ciudad", Valor = ciudades[i], Orden = i, Activo = true, FechaCreacion = now });

        var clases = procesos.Select(p => p.ClaseProceso).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c).ToList();
        for (int i = 0; i < clases.Count; i++)
            catalogosToInsert.Add(new Catalogo { Tipo = "claseProceso", Valor = clases[i], Orden = i, Activo = true, FechaCreacion = now });

        var ingresados = procesos.Select(p => p.ProcesoIngresadoPor).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c!).ToList();
        for (int i = 0; i < ingresados.Count; i++)
            catalogosToInsert.Add(new Catalogo { Tipo = "ingresadoPor", Valor = ingresados[i]!, Orden = i, Activo = true, FechaCreacion = now });

        if (catalogosToInsert.Count > 0)
        {
            // Clear existing catalogos first
            await _db.Catalogos.DeleteManyAsync(FilterDefinition<Catalogo>.Empty);
            await _db.Catalogos.InsertManyAsync(catalogosToInsert);
        }

        var totalPagos = procesos.Sum(p => p.Pagos.Count);
        var totalNotas = procesos.Sum(p => p.Notas.Count);
        var totalRecordatorios = procesos.Sum(p => p.Recordatorios.Count);

        return Ok(new
        {
            mensaje = "Migracion completada exitosamente",
            procesos = procesos.Count,
            pagos = totalPagos,
            notas = totalNotas,
            recordatorios = totalRecordatorios,
            catalogos = catalogosToInsert.Count,
            detalleCatalogos = new
            {
                ciudades = ciudades.Count,
                clasesProceso = clases.Count,
                ingresadoPor = ingresados.Count,
            }
        });
    }

    [HttpPost("normalizar-textos")]
    public async Task<IActionResult> NormalizarTextos()
    {
        var procesos = await _db.Procesos.Find(FilterDefinition<Proceso>.Empty).ToListAsync();
        var actualizados = 0;

        foreach (var p in procesos)
        {
            var update = Builders<Proceso>.Update
                .Set(x => x.Demandante, ToTitleCase(p.Demandante))
                .Set(x => x.Demandado, ToTitleCase(p.Demandado))
                .Set(x => x.Juzgado, ToTitleCase(p.Juzgado))
                .Set(x => x.Ciudad, ToTitleCase(p.Ciudad))
                .Set(x => x.ClaseProceso, ToTitleCase(p.ClaseProceso))
                .Set(x => x.Representamos, ToTitleCase(p.Representamos))
                .Set(x => x.ProcesoIngresadoPor, ToTitleCase(p.ProcesoIngresadoPor));

            await _db.Procesos.UpdateOneAsync(x => x.Id == p.Id, update);
            actualizados++;
        }

        // Also normalize catalogo values
        var catalogos = await _db.Catalogos.Find(FilterDefinition<Catalogo>.Empty).ToListAsync();
        var catalogosActualizados = 0;

        foreach (var c in catalogos)
        {
            var nuevoValor = ToTitleCase(c.Valor);
            if (nuevoValor != c.Valor)
            {
                await _db.Catalogos.UpdateOneAsync(
                    x => x.Id == c.Id,
                    Builders<Catalogo>.Update.Set(x => x.Valor, nuevoValor));
                catalogosActualizados++;
            }
        }

        return Ok(new
        {
            mensaje = "Textos normalizados a Title Case",
            procesosActualizados = actualizados,
            catalogosActualizados
        });
    }

    private static string? ToTitleCase(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
    }

    #region SQLite Helpers

    private static string GetString(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }

    private static string? GetNullableString(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static DateTime GetDateTime(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal)) return DateTime.UtcNow;
        var value = reader.GetString(ordinal);
        return DateTime.TryParse(value, out var dt) ? dt : DateTime.UtcNow;
    }

    private static bool GetBool(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal)) return true;
        return reader.GetInt64(ordinal) != 0;
    }

    private static decimal GetDecimal(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        if (reader.IsDBNull(ordinal)) return 0m;
        return reader.GetDecimal(ordinal);
    }

    #endregion
}

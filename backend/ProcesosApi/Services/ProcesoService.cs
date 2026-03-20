using MongoDB.Bson;
using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;
using ProcesosApi.Models;

namespace ProcesosApi.Services;

public class ProcesoService : IProcesoService
{
    private readonly MongoDbContext _db;

    public ProcesoService(MongoDbContext db) => _db = db;

    public async Task<PaginatedResultDto<ProcesoListItemDto>> ListarAsync(
        string? busqueda, string? ciudad, string? claseProceso,
        string? estado, string? ingresadoPor,
        string? ordenarPor, bool ordenDescendente,
        int pagina, int porPagina)
    {
        var builder = Builders<Proceso>.Filter;
        var filter = builder.Eq(p => p.Activo, true);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var regex = new BsonRegularExpression(busqueda, "i");
            filter &= builder.Or(
                builder.Regex(p => p.Demandante, regex),
                builder.Regex(p => p.Demandado, regex),
                builder.Regex(p => p.Radicado, regex),
                builder.Regex(p => p.Juzgado, regex));
        }

        if (!string.IsNullOrWhiteSpace(ciudad))
            filter &= builder.Eq(p => p.Ciudad, ciudad);
        if (!string.IsNullOrWhiteSpace(claseProceso))
            filter &= builder.Eq(p => p.ClaseProceso, claseProceso);
        if (!string.IsNullOrWhiteSpace(estado))
            filter &= builder.Regex(p => p.EstadoActual, new BsonRegularExpression(estado, "i"));
        if (!string.IsNullOrWhiteSpace(ingresadoPor))
            filter &= builder.Eq(p => p.ProcesoIngresadoPor, ingresadoPor);

        var sortBuilder = Builders<Proceso>.Sort;
        var sort = (ordenarPor?.ToLower()) switch
        {
            "demandante" => ordenDescendente ? sortBuilder.Descending(p => p.Demandante) : sortBuilder.Ascending(p => p.Demandante),
            "ciudad" => ordenDescendente ? sortBuilder.Descending(p => p.Ciudad) : sortBuilder.Ascending(p => p.Ciudad),
            "claseproceso" => ordenDescendente ? sortBuilder.Descending(p => p.ClaseProceso) : sortBuilder.Ascending(p => p.ClaseProceso),
            "radicado" => ordenDescendente ? sortBuilder.Descending(p => p.Radicado) : sortBuilder.Ascending(p => p.Radicado),
            _ => ordenDescendente ? sortBuilder.Descending(p => p.Fecha) : sortBuilder.Ascending(p => p.Fecha),
        };

        var totalCount = await _db.Procesos.CountDocumentsAsync(filter);

        var procesos = await _db.Procesos
            .Find(filter)
            .Sort(sort)
            .Skip((pagina - 1) * porPagina)
            .Limit(porPagina)
            .ToListAsync();

        var items = procesos.Select(p => new ProcesoListItemDto
        {
            Id = p.Id,
            Fecha = p.Fecha,
            Demandante = p.Demandante,
            Demandado = p.Demandado,
            Radicado = p.Radicado,
            Ciudad = p.Ciudad,
            ClaseProceso = p.ClaseProceso,
            EstadoActual = p.EstadoActual,
            ProcesoIngresadoPor = p.ProcesoIngresadoPor,
            Terminado = p.Terminado,
            CantidadPagos = p.Pagos.Count,
            CantidadNotas = p.Notas.Count,
            RecordatoriosPendientes = p.Recordatorios.Count(r => !r.Completado)
        }).ToList();

        return new PaginatedResultDto<ProcesoListItemDto>
        {
            Items = items,
            TotalCount = (int)totalCount,
            Pagina = pagina,
            PorPagina = porPagina
        };
    }

    public async Task<ProcesoDetalleDto?> ObtenerPorIdAsync(string id)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, id) &
                     Builders<Proceso>.Filter.Eq(p => p.Activo, true);

        var p = await _db.Procesos.Find(filter).FirstOrDefaultAsync();
        if (p == null) return null;

        return MapToDetalle(p);
    }

    public async Task<ProcesoDetalleDto> CrearAsync(ProcesoCreateDto dto)
    {
        var proceso = new Proceso
        {
            Fecha = dto.Fecha,
            Demandante = dto.Demandante,
            Demandado = dto.Demandado,
            Radicado = dto.Radicado,
            Juzgado = dto.Juzgado,
            Ciudad = dto.Ciudad,
            ClaseProceso = dto.ClaseProceso,
            Representamos = dto.Representamos,
            ProcesoIngresadoPor = dto.ProcesoIngresadoPor,
            Honorarios = dto.Honorarios,
            Comisiones = dto.Comisiones.Select(c => new Comision
            {
                Persona = c.Persona,
                Porcentaje = c.Porcentaje
            }).ToList(),
            EstadoActual = dto.EstadoActual,
            CamposGlobales = dto.CamposGlobales ?? new(),
            CamposPropios = (dto.CamposPropios ?? new()).Select(c => new CampoPropio
            {
                Nombre = c.Nombre,
                Valor = c.Valor
            }).ToList(),
            Terminado = dto.Terminado,
            NotaTerminacion = dto.NotaTerminacion,
            FechaTerminacion = dto.Terminado ? DateTime.UtcNow : null,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _db.Procesos.InsertOneAsync(proceso);

        return MapToDetalle(proceso);
    }

    public async Task<ProcesoDetalleDto?> ActualizarAsync(string id, ProcesoUpdateDto dto)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, id) &
                     Builders<Proceso>.Filter.Eq(p => p.Activo, true);

        var update = Builders<Proceso>.Update
            .Set(p => p.Fecha, dto.Fecha)
            .Set(p => p.Demandante, dto.Demandante)
            .Set(p => p.Demandado, dto.Demandado)
            .Set(p => p.Radicado, dto.Radicado)
            .Set(p => p.Juzgado, dto.Juzgado)
            .Set(p => p.Ciudad, dto.Ciudad)
            .Set(p => p.ClaseProceso, dto.ClaseProceso)
            .Set(p => p.Representamos, dto.Representamos)
            .Set(p => p.ProcesoIngresadoPor, dto.ProcesoIngresadoPor)
            .Set(p => p.Honorarios, dto.Honorarios)
            .Set(p => p.Comisiones, dto.Comisiones.Select(c => new Comision
            {
                Persona = c.Persona,
                Porcentaje = c.Porcentaje
            }).ToList())
            .Set(p => p.EstadoActual, dto.EstadoActual)
            .Set(p => p.CamposGlobales, dto.CamposGlobales ?? new())
            .Set(p => p.CamposPropios, (dto.CamposPropios ?? new()).Select(c => new CampoPropio
            {
                Nombre = c.Nombre,
                Valor = c.Valor
            }).ToList())
            .Set(p => p.Terminado, dto.Terminado)
            .Set(p => p.NotaTerminacion, dto.NotaTerminacion)
            .Set(p => p.FechaTerminacion, dto.Terminado ? DateTime.UtcNow : null)
            .Set(p => p.FechaActualizacion, DateTime.UtcNow);

        var result = await _db.Procesos.FindOneAndUpdateAsync(
            filter, update,
            new FindOneAndUpdateOptions<Proceso> { ReturnDocument = ReturnDocument.After });

        if (result == null) return null;
        return MapToDetalle(result);
    }

    public async Task<bool> EliminarAsync(string id)
    {
        var filter = Builders<Proceso>.Filter.Eq(p => p.Id, id) &
                     Builders<Proceso>.Filter.Eq(p => p.Activo, true);

        var update = Builders<Proceso>.Update
            .Set(p => p.Activo, false)
            .Set(p => p.FechaActualizacion, DateTime.UtcNow);

        var result = await _db.Procesos.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    private static ProcesoDetalleDto MapToDetalle(Proceso p) => new()
    {
        Id = p.Id,
        Fecha = p.Fecha,
        Demandante = p.Demandante,
        Demandado = p.Demandado,
        Radicado = p.Radicado,
        Juzgado = p.Juzgado,
        Ciudad = p.Ciudad,
        ClaseProceso = p.ClaseProceso,
        Representamos = p.Representamos,
        ProcesoIngresadoPor = p.ProcesoIngresadoPor,
        Honorarios = p.Honorarios,
        Comisiones = p.Comisiones.Select(c => new ComisionDto
        {
            Persona = c.Persona,
            Porcentaje = c.Porcentaje
        }).ToList(),
        EstadoActual = p.EstadoActual,
        Terminado = p.Terminado,
        NotaTerminacion = p.NotaTerminacion,
        FechaTerminacion = p.FechaTerminacion,
        CamposGlobales = p.CamposGlobales ?? new(),
        CamposPropios = (p.CamposPropios ?? new()).Select(c => new CampoPropioDto
        {
            Nombre = c.Nombre,
            Valor = c.Valor
        }).ToList(),
        FechaCreacion = p.FechaCreacion,
        FechaActualizacion = p.FechaActualizacion,
        Pagos = p.Pagos.OrderByDescending(pa => pa.Fecha).Select(pa => new PagoDto
        {
            Id = pa.Id,
            ProcesoId = p.Id,
            Fecha = pa.Fecha,
            Monto = pa.Monto,
            Concepto = pa.Concepto,
            MetodoPago = pa.MetodoPago,
            FechaCreacion = pa.FechaCreacion
        }).ToList(),
        Notas = p.Notas.OrderByDescending(n => n.FechaCreacion).Select(n => new NotaDto
        {
            Id = n.Id,
            ProcesoId = p.Id,
            Contenido = n.Contenido,
            FechaCreacion = n.FechaCreacion,
            FechaActualizacion = n.FechaActualizacion
        }).ToList(),
        Recordatorios = p.Recordatorios.OrderBy(r => r.FechaVencimiento).Select(r => new RecordatorioDto
        {
            Id = r.Id,
            ProcesoId = p.Id,
            Titulo = r.Titulo,
            Descripcion = r.Descripcion,
            FechaVencimiento = r.FechaVencimiento,
            Completado = r.Completado,
            FechaCreacion = r.FechaCreacion
        }).ToList()
    };
}

using MongoDB.Driver;
using ProcesosApi.Data;
using ProcesosApi.DTOs;

namespace ProcesosApi.Services;

public class DashboardService : IDashboardService
{
    private readonly MongoDbContext _db;

    public DashboardService(MongoDbContext db) => _db = db;

    public async Task<DashboardDto> ObtenerEstadisticasAsync()
    {
        var today = DateTime.UtcNow.Date;
        var activeFilter = Builders<Models.Proceso>.Filter.Eq(p => p.Activo, true);

        var procesos = await _db.Procesos.Find(activeFilter).ToListAsync();

        var totalProcesos = procesos.Count;

        var procesosPorTipo = procesos
            .GroupBy(p => p.ClaseProceso)
            .Select(g => new ConteoItemDto { Nombre = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad)
            .ToList();

        var procesosPorCiudad = procesos
            .GroupBy(p => p.Ciudad)
            .Select(g => new ConteoItemDto { Nombre = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad)
            .ToList();

        var procesosPorIngresadoPor = procesos
            .Where(p => !string.IsNullOrEmpty(p.ProcesoIngresadoPor))
            .GroupBy(p => p.ProcesoIngresadoPor!)
            .Select(g => new ConteoItemDto { Nombre = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad)
            .ToList();

        var allRecordatorios = procesos.SelectMany(p =>
            p.Recordatorios.Select(r => new { Recordatorio = r, Proceso = p })).ToList();

        var recordatoriosPendientes = allRecordatorios
            .Count(r => !r.Recordatorio.Completado && r.Recordatorio.FechaVencimiento.Date >= today);

        var recordatoriosVencidos = allRecordatorios
            .Count(r => !r.Recordatorio.Completado && r.Recordatorio.FechaVencimiento.Date < today);

        var proximosRecordatorios = allRecordatorios
            .Where(r => !r.Recordatorio.Completado)
            .OrderBy(r => r.Recordatorio.FechaVencimiento)
            .Take(5)
            .Select(r => new RecordatorioDto
            {
                Id = r.Recordatorio.Id,
                ProcesoId = r.Proceso.Id,
                Titulo = r.Recordatorio.Titulo,
                Descripcion = r.Recordatorio.Descripcion,
                FechaVencimiento = r.Recordatorio.FechaVencimiento,
                Completado = r.Recordatorio.Completado,
                FechaCreacion = r.Recordatorio.FechaCreacion,
                DemandanteDelProceso = r.Proceso.Demandante,
                RadicadoDelProceso = r.Proceso.Radicado
            })
            .ToList();

        return new DashboardDto
        {
            TotalProcesos = totalProcesos,
            ProcesosActivos = totalProcesos,
            RecordatoriosPendientes = recordatoriosPendientes,
            RecordatoriosVencidos = recordatoriosVencidos,
            ProcesosPorTipo = procesosPorTipo,
            ProcesosPorCiudad = procesosPorCiudad,
            ProcesosPorIngresadoPor = procesosPorIngresadoPor,
            ProximosRecordatorios = proximosRecordatorios
        };
    }

    public async Task<FiltrosDisponiblesDto> ObtenerFiltrosAsync()
    {
        var procesos = await _db.Procesos
            .Find(p => p.Activo)
            .ToListAsync();

        return new FiltrosDisponiblesDto
        {
            Ciudades = procesos.Select(p => p.Ciudad).Distinct().OrderBy(x => x).ToList(),
            ClasesProceso = procesos.Select(p => p.ClaseProceso).Distinct().OrderBy(x => x).ToList(),
            IngresadoPor = procesos
                .Where(p => !string.IsNullOrEmpty(p.ProcesoIngresadoPor))
                .Select(p => p.ProcesoIngresadoPor!).Distinct().OrderBy(x => x).ToList(),
            Estados = procesos
                .Where(p => !string.IsNullOrEmpty(p.EstadoActual))
                .Select(p => p.EstadoActual!).Distinct().OrderBy(x => x).ToList()
        };
    }
}

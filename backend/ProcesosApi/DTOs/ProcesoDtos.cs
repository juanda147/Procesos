using ProcesosApi.Models;

namespace ProcesosApi.DTOs;

public class ProcesoListItemDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Demandante { get; set; } = string.Empty;
    public string Demandado { get; set; } = string.Empty;
    public string Radicado { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string ClaseProceso { get; set; } = string.Empty;
    public string? EstadoActual { get; set; }
    public string? ProcesoIngresadoPor { get; set; }
    public bool Terminado { get; set; }
    public int CantidadPagos { get; set; }
    public int CantidadNotas { get; set; }
    public int RecordatoriosPendientes { get; set; }
}

public class ProcesoDetalleDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Demandante { get; set; } = string.Empty;
    public string Demandado { get; set; } = string.Empty;
    public string Radicado { get; set; } = string.Empty;
    public string Juzgado { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string ClaseProceso { get; set; } = string.Empty;
    public string? Representamos { get; set; }
    public string? ProcesoIngresadoPor { get; set; }
    public string? Honorarios { get; set; }
    public List<ComisionDto> Comisiones { get; set; } = new();
    public string? EstadoActual { get; set; }
    public Dictionary<string, string> CamposGlobales { get; set; } = new();
    public List<CampoPropioDto> CamposPropios { get; set; } = new();
    public bool Terminado { get; set; }
    public string? NotaTerminacion { get; set; }
    public DateTime? FechaTerminacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public List<PagoDto> Pagos { get; set; } = new();
    public List<NotaDto> Notas { get; set; } = new();
    public List<RecordatorioDto> Recordatorios { get; set; } = new();
}

public class ComisionDto
{
    public string Persona { get; set; } = string.Empty;
    public string Porcentaje { get; set; } = string.Empty;
}

public class ProcesoCreateDto
{
    public DateTime Fecha { get; set; }
    public string Demandante { get; set; } = string.Empty;
    public string Demandado { get; set; } = string.Empty;
    public string Radicado { get; set; } = string.Empty;
    public string Juzgado { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string ClaseProceso { get; set; } = string.Empty;
    public string? Representamos { get; set; }
    public string? ProcesoIngresadoPor { get; set; }
    public string? Honorarios { get; set; }
    public List<ComisionDto> Comisiones { get; set; } = new();
    public string? EstadoActual { get; set; }
    public Dictionary<string, string> CamposGlobales { get; set; } = new();
    public List<CampoPropioDto> CamposPropios { get; set; } = new();
    public bool Terminado { get; set; }
    public string? NotaTerminacion { get; set; }
}

public class ProcesoUpdateDto : ProcesoCreateDto { }

public class PaginatedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Pagina { get; set; }
    public int PorPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling(TotalCount / (double)PorPagina);
}

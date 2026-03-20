namespace ProcesosApi.DTOs;

public class DashboardDto
{
    public int TotalProcesos { get; set; }
    public int ProcesosActivos { get; set; }
    public int RecordatoriosPendientes { get; set; }
    public int RecordatoriosVencidos { get; set; }
    public List<ConteoItemDto> ProcesosPorTipo { get; set; } = new();
    public List<ConteoItemDto> ProcesosPorCiudad { get; set; } = new();
    public List<ConteoItemDto> ProcesosPorIngresadoPor { get; set; } = new();
    public List<RecordatorioDto> ProximosRecordatorios { get; set; } = new();
}

public class ConteoItemDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class FiltrosDisponiblesDto
{
    public List<string> Ciudades { get; set; } = new();
    public List<string> ClasesProceso { get; set; } = new();
    public List<string> IngresadoPor { get; set; } = new();
    public List<string> Estados { get; set; } = new();
}

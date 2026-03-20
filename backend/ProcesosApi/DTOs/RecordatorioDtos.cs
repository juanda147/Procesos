namespace ProcesosApi.DTOs;

public class RecordatorioDto
{
    public string Id { get; set; } = string.Empty;
    public string ProcesoId { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool Completado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? CorreoNotificacion { get; set; }
    public string? DemandanteDelProceso { get; set; }
    public string? RadicadoDelProceso { get; set; }
}

public class RecordatorioCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string? CorreoNotificacion { get; set; }
}

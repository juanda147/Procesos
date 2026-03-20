namespace ProcesosApi.DTOs;

public class NotaDto
{
    public string Id { get; set; } = string.Empty;
    public string ProcesoId { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

public class NotaCreateDto
{
    public string Contenido { get; set; } = string.Empty;
}

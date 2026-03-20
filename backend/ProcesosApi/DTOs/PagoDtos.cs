namespace ProcesosApi.DTOs;

public class PagoDto
{
    public string Id { get; set; } = string.Empty;
    public string ProcesoId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
    public string? MetodoPago { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class PagoCreateDto
{
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
    public string? MetodoPago { get; set; }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProcesosApi.Models;

public class Proceso
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
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
    public List<Comision> Comisiones { get; set; } = new();
    public string? EstadoActual { get; set; }
    public Dictionary<string, string> CamposGlobales { get; set; } = new();
    public List<CampoPropio> CamposPropios { get; set; } = new();
    public bool Terminado { get; set; }
    public string? NotaTerminacion { get; set; }
    public DateTime? FechaTerminacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public bool Activo { get; set; } = true;

    public List<Pago> Pagos { get; set; } = new();
    public List<Nota> Notas { get; set; } = new();
    public List<Recordatorio> Recordatorios { get; set; } = new();
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProcesosApi.Models;

public class Recordatorio
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool Completado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string? CorreoNotificacion { get; set; }
    public bool NotificacionEnviada { get; set; }
}

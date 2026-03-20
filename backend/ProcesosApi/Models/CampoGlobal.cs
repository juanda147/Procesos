using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProcesosApi.Models;

public class CampoGlobal
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = "texto"; // "texto", "fecha", "numero"
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProcesosApi.Models;

public class Catalogo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
}

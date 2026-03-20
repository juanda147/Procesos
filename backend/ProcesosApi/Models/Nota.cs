using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProcesosApi.Models;

public class Nota
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Contenido { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProcesosApi.Models;

public class Pago
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
    public string? MetodoPago { get; set; }
    public DateTime FechaCreacion { get; set; }
}

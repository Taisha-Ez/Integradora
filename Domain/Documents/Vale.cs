using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fenixjobs_api.Domain.Documents
{
    public class Vale
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }

        [BsonElement("UserId")]
        public int UserId { get; set; }

        [BsonElement("Usuario")]
        public string Usuario { get; set; } = string.Empty;

        [BsonElement("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("ApellidoPaterno")]
        public string? ApellidoPaterno { get; set; }

        [BsonElement("ApellidoMaterno")]
        public string? ApellidoMaterno { get; set; }

        [BsonElement("TipoUsuario")]
        public string TipoUsuario { get; set; } = string.Empty;

        [BsonElement("MontoSolicitado")]
        public decimal MontoSolicitado { get; set; }

        [BsonElement("PlazoPagoMeses")]
        public int PlazoPagoMeses { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; } = "Pendiente";

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

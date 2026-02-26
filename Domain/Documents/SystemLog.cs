using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fenixjobs_api.Domain.Documents
{
    public class SystemLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }

        [BsonElement("Action")]
        public string Action { get; set; } = string.Empty;

        [BsonElement("Email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("Details")]
        public string? Details { get; set; }

        [BsonElement("IpAddress")]
        public string? IpAddress { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

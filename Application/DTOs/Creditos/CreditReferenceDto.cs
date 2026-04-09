using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Creditos
{
    public class CreditReferenceDto
    {
        [JsonPropertyName("parentesco")]
        public string Relationship { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("numero_contacto")]
        public string ContactPhone { get; set; } = string.Empty;
    }
}
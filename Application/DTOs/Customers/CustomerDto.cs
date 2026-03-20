using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Customers
{
    public class CustomerDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("id_user")]
        public int IdUser { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; } = string.Empty;
    }
}

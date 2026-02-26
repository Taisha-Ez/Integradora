using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs
{
    public class LoginDto
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; } = string.Empty;

        [JsonPropertyName("contrasenia")]
        public string Contrasenia { get; set; } = string.Empty;
    }
}
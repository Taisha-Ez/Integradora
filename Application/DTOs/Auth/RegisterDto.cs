using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("apellido_paterno")]
        public string? ApellidoPaterno { get; set; }

        [JsonPropertyName("apellido_materno")]
        public string? ApellidoMaterno { get; set; }

        [JsonPropertyName("usuario")]
        public string Usuario { get; set; } = string.Empty;

        [JsonPropertyName("contrasenia")]
        public string Contrasenia { get; set; } = string.Empty;

        [JsonPropertyName("tipo_usuario")]
        public string? TipoUsuario { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Vales
{
    public class ResolveValeStatusDto
    {
        [JsonPropertyName("status")]
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
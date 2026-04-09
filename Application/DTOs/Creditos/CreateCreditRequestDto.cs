using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Creditos
{
    public class CreateCreditRequestDto
    {
        [JsonPropertyName("nombre_completo")]
        [Required]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("curp_rfc")]
        [Required]
        public string CurpRfc { get; set; } = string.Empty;

        [JsonPropertyName("direccion")]
        [Required]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("telefono")]
        [Required]
        public string Phone { get; set; } = string.Empty;

        [JsonPropertyName("ingresos_mensuales")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Los ingresos mensuales deben ser mayores a 0.")]
        public decimal MonthlyIncome { get; set; }

        [JsonPropertyName("referencias")]
        [Required]
        [MinLength(2, ErrorMessage = "Se requieren al menos 2 referencias.")]
        public List<CreditReferenceDto> References { get; set; } = new();
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Vales
{
    public class PayValeDto
    {
        [JsonPropertyName("monto_pago")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto de pago debe ser mayor a 0.")]
        public decimal MontoPago { get; set; }
    }
}
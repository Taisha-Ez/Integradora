using System.Text.Json.Serialization;

namespace fenixjobs_api.Application.DTOs.Vales
{
    public class CreateValeDto
    {
        [JsonPropertyName("monto_solicitar")]
        public decimal MontoSolicitar { get; set; }

        [JsonPropertyName("plazo_pago_meses")]
        public int PlazoPagoMeses { get; set; }
    }
}

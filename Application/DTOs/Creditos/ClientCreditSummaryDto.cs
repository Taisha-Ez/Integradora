namespace fenixjobs_api.Application.DTOs.Creditos
{
    public class ClientCreditSummaryDto
    {
        public int UserId { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;

        public int CreditRequestId { get; set; }

        public string CurpRfc { get; set; } = string.Empty;

        public decimal MonthlyIncome { get; set; }

        public decimal EstimatedCredit { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
namespace fenixjobs_api.Application.DTOs.Creditos
{
    public class CreditBalanceDto
    {
        public int CreditRequestId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public decimal SaldoDisponible { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
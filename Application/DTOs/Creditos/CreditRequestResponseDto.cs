namespace fenixjobs_api.Application.DTOs.Creditos
{
    public class CreditRequestResponseDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string CurpRfc { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public decimal MonthlyIncome { get; set; }

        public decimal EstimatedCredit { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<CreditReferenceDto> References { get; set; } = new();
    }
}
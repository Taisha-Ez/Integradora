using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fenixjobs_api.Domain.Entities
{
    [Table("solicitudes_credito")]
    public class CreditRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string CurpRfc { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedCredit { get; set; }

        [Required]
        public string Status { get; set; } = "Estimado";

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Users User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CreditReference> References { get; set; } = new List<CreditReference>();
    }
}
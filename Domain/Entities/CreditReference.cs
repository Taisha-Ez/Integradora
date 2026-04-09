using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fenixjobs_api.Domain.Entities
{
    [Table("solicitudes_credito_referencias")]
    public class CreditReference
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Relationship { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        public int CreditRequestId { get; set; }

        [ForeignKey(nameof(CreditRequestId))]
        public CreditRequest CreditRequest { get; set; } = null!;
    }
}
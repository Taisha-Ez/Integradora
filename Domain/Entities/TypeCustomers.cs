using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fenixjobs_api.Domain.Entities
{
    [Table("type_customers")]
    public class TypeCustomers
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int id_user { get; set; }

        [ForeignKey(nameof(id_user))]
        public Users User { get; set; }
    }
}
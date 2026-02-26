using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fenixjobs_api.Domain.Entities
{
    [Table("usuarios")]
    public class Users
    {
        [Key]
        public int id_usuario { get; set; }

        [Required]
        public string nombre { get; set; }

        public string? apellido_paterno { get; set; }

        public string? apellido_materno { get; set; }

        [Required]
        public string usuario { get; set; }

        [Required]
        public string contrasenia { get; set; }

        public string tipo_usuario { get; set; } = "cliente";
    }
}
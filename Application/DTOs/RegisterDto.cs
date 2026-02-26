namespace fenixjobs_api.Application.DTOs
{
    public class RegisterDto
    {
        public string Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Usuario { get; set; }
        public string Contrasenia { get; set; }
        public string? TipoUsuario { get; set; }
    }
}
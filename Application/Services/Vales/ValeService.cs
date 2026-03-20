using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Vales;
using fenixjobs_api.Application.Interfaces.Auth;
using fenixjobs_api.Application.Interfaces.Vales;
using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Services.Vales
{
    public class ValeService : IValeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValeRepository _valeRepository;

        public ValeService(IUserRepository userRepository, IValeRepository valeRepository)
        {
            _userRepository = userRepository;
            _valeRepository = valeRepository;
        }

        public async Task<ServiceResponseDto<Vale>> CreateForClientAsync(int userId, CreateValeDto dto)
        {
            var response = new ServiceResponseDto<Vale>();

            if (dto.MontoSolicitar <= 0)
            {
                response.Status = false;
                response.Message = "El monto a solicitar debe ser mayor a 0.";
                return response;
            }

            if (dto.PlazoPagoMeses <= 0)
            {
                response.Status = false;
                response.Message = "El plazo de pago en meses debe ser mayor a 0.";
                return response;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Status = false;
                response.Message = "Usuario no encontrado.";
                return response;
            }

            if (!string.Equals(user.tipo_usuario, "cliente", StringComparison.OrdinalIgnoreCase))
            {
                response.Status = false;
                response.Message = "Solo los usuarios cliente pueden solicitar vales.";
                return response;
            }

            var vale = new Vale
            {
                UserId = user.id_usuario,
                Usuario = user.usuario,
                Nombre = user.nombre,
                ApellidoPaterno = user.apellido_paterno,
                ApellidoMaterno = user.apellido_materno,
                TipoUsuario = user.tipo_usuario,
                MontoSolicitado = dto.MontoSolicitar,
                PlazoPagoMeses = dto.PlazoPagoMeses,
                Status = "Pendiente",
                CreatedAt = DateTime.UtcNow
            };

            await _valeRepository.CreateAsync(vale);

            response.Data = vale;
            response.Message = "Vale creado exitosamente con status Pendiente.";
            return response;
        }
    }
}

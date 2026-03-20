using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Vales;
using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Application.Interfaces.Auth;
using fenixjobs_api.Application.Interfaces.Vales;
using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Services.Vales
{
    public class ValeService : IValeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValeRepository _valeRepository;
        private readonly ISystemLogRepository _logRepository;

        public ValeService(IUserRepository userRepository, IValeRepository valeRepository, ISystemLogRepository logRepository)
        {
            _userRepository = userRepository;
            _valeRepository = valeRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResponseDto<Vale>> CreateForClientAsync(int userId, CreateValeDto dto, string? actorUser = null)
        {
            var response = new ServiceResponseDto<Vale>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? userId.ToString() : actorUser;

            if (dto.MontoSolicitar <= 0)
            {
                response.Status = false;
                response.Message = "El monto a solicitar debe ser mayor a 0.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = "Solicitud de vale rechazada por monto invalido.",
                    CreatedAt = DateTime.UtcNow
                });
                return response;
            }

            if (dto.PlazoPagoMeses <= 0)
            {
                response.Status = false;
                response.Message = "El plazo de pago en meses debe ser mayor a 0.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = "Solicitud de vale rechazada por plazo invalido.",
                    CreatedAt = DateTime.UtcNow
                });
                return response;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Status = false;
                response.Message = "Usuario no encontrado.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = "Solicitud de vale rechazada. Usuario no encontrado.",
                    CreatedAt = DateTime.UtcNow
                });
                return response;
            }

            if (!string.Equals(user.tipo_usuario, "cliente", StringComparison.OrdinalIgnoreCase))
            {
                response.Status = false;
                response.Message = "Solo los usuarios cliente pueden solicitar vales.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = $"Solicitud de vale rechazada por rol invalido: {user.tipo_usuario}",
                    CreatedAt = DateTime.UtcNow
                });
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

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "Vales.Create",
                User = logUser,
                Details = $"Vale creado exitosamente. Monto: {dto.MontoSolicitar}, Plazo: {dto.PlazoPagoMeses}, Status: Pendiente.",
                CreatedAt = DateTime.UtcNow
            });

            return response;
        }

        public async Task<ServiceResponseDto<List<Vale>>> GetAllAsync(string? status = null, string? actorUser = null)
        {
            var response = new ServiceResponseDto<List<Vale>>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? "unknown" : actorUser;

            try
            {
                response.Data = await _valeRepository.GetAllAsync(status);
                response.Message = "Vales obtenidos exitosamente.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.GetAll",
                    User = logUser,
                    Details = $"Consulta de vales ejecutada. Filtro status: {status ?? "Todos"}. Total: {response.Data?.Count ?? 0}",
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error al obtener vales: " + ex.Message;

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.GetAll",
                    User = logUser,
                    Details = $"Error al consultar vales. Filtro status: {status ?? "Todos"}. Error: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return response;
        }
    }
}

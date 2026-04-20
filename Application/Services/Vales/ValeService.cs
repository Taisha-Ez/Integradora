using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Vales;
using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Application.Interfaces.Auth;
using fenixjobs_api.Application.Interfaces.Creditos;
using fenixjobs_api.Application.Interfaces.Vales;
using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Services.Vales
{
    public class ValeService : IValeService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreditRequestRepository _creditRequestRepository;
        private readonly IValeRepository _valeRepository;
        private readonly ISystemLogRepository _logRepository;

        public ValeService(IUserRepository userRepository, ICreditRequestRepository creditRequestRepository, IValeRepository valeRepository, ISystemLogRepository logRepository)
        {
            _userRepository = userRepository;
            _creditRequestRepository = creditRequestRepository;
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

            if (dto.MontoPagoMensual <= 0)
            {
                response.Status = false;
                response.Message = "El monto de pago mensual debe ser mayor a 0.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = "Solicitud de vale rechazada por monto mensual invalido.",
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

            var creditRequest = await _creditRequestRepository.GetActiveByUserIdAsync(user.id_usuario);
            if (creditRequest == null)
            {
                response.Status = false;
                response.Message = "No cuentas con un credito autorizado activo para solicitar vales.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = "Solicitud de vale rechazada. Sin credito autorizado activo.",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            if (dto.MontoSolicitar > creditRequest.EstimatedCredit)
            {
                response.Status = false;
                response.Message = $"Monto insuficiente. Credito disponible: {creditRequest.EstimatedCredit}.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Create",
                    User = logUser,
                    Details = $"Solicitud de vale rechazada por saldo insuficiente. Solicitado: {dto.MontoSolicitar}, Disponible: {creditRequest.EstimatedCredit}",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            creditRequest.EstimatedCredit -= dto.MontoSolicitar;
            await _creditRequestRepository.UpdateAsync(creditRequest);

            var vale = new Vale
            {
                UserId = user.id_usuario,
                Usuario = user.usuario,
                Nombre = user.nombre,
                ApellidoPaterno = user.apellido_paterno,
                ApellidoMaterno = user.apellido_materno,
                TipoUsuario = user.tipo_usuario,
                MontoSolicitado = dto.MontoSolicitar,
                MontoRestante = dto.MontoSolicitar,
                PlazoPagoMeses = dto.PlazoPagoMeses,
                MontoPagoMensual = dto.MontoPagoMensual,
                Status = "Pendiente",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _valeRepository.CreateAsync(vale);
            }
            catch
            {
                creditRequest.EstimatedCredit += dto.MontoSolicitar;
                await _creditRequestRepository.UpdateAsync(creditRequest);
                throw;
            }

            response.Data = vale;
            response.Message = $"Vale creado exitosamente con status Pendiente. Credito restante: {creditRequest.EstimatedCredit}.";

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "Vales.Create",
                User = logUser,
                Details = $"Vale creado exitosamente. Monto: {dto.MontoSolicitar}, Plazo: {dto.PlazoPagoMeses}, Pago mensual: {dto.MontoPagoMensual}, Status: Pendiente, Credito restante: {creditRequest.EstimatedCredit}.",
                CreatedAt = DateTime.UtcNow
            });

            return response;
        }

        public async Task<ServiceResponseDto<Vale>> PayAsync(int userId, string valeId, PayValeDto dto, string? actorUser = null)
        {
            var response = new ServiceResponseDto<Vale>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? userId.ToString() : actorUser;

            if (dto.MontoPago <= 0)
            {
                response.Status = false;
                response.Message = "El monto de pago debe ser mayor a 0.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Pay",
                    User = logUser,
                    Details = "Pago de vale rechazado por monto invalido.",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Status = false;
                response.Message = "Usuario no encontrado.";
                return response;
            }

            var vale = await _valeRepository.GetByIdAsync(valeId);
            if (vale == null)
            {
                response.Status = false;
                response.Message = "Vale no encontrado.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.Pay",
                    User = logUser,
                    Details = $"Pago de vale rechazado. Vale no encontrado: {valeId}.",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            if (vale.UserId != user.id_usuario)
            {
                response.Status = false;
                response.Message = "No puedes pagar un vale que no te pertenece.";
                return response;
            }

            if (!string.Equals(vale.Status, "Aceptado", StringComparison.OrdinalIgnoreCase))
            {
                response.Status = false;
                response.Message = "Solo se pueden pagar vales aceptados por el admin.";
                return response;
            }

            var saldoPendiente = vale.MontoRestante > 0 ? vale.MontoRestante : vale.MontoSolicitado;
            if (dto.MontoPago > saldoPendiente)
            {
                response.Status = false;
                response.Message = $"El pago excede el saldo pendiente. Saldo pendiente: {saldoPendiente}.";
                return response;
            }

            var creditRequest = await _creditRequestRepository.GetActiveByUserIdAsync(user.id_usuario);
            if (creditRequest == null)
            {
                response.Status = false;
                response.Message = "No se encontro un credito autorizado activo para reembolsar el pago.";
                return response;
            }

            creditRequest.EstimatedCredit += dto.MontoPago;
            await _creditRequestRepository.UpdateAsync(creditRequest);

            vale.MontoRestante = saldoPendiente - dto.MontoPago;
            vale.Status = vale.MontoRestante <= 0 ? "Pagado" : "Aceptado";

            try
            {
                await _valeRepository.UpdateAsync(vale);
            }
            catch
            {
                creditRequest.EstimatedCredit -= dto.MontoPago;
                await _creditRequestRepository.UpdateAsync(creditRequest);
                throw;
            }

            response.Data = vale;
            response.Message = $"Pago aplicado correctamente. Saldo restante del vale: {vale.MontoRestante}. Credito autorizado actualizado: {creditRequest.EstimatedCredit}.";

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "Vales.Pay",
                User = logUser,
                Details = $"Pago de vale aplicado. Vale: {valeId}, Pago: {dto.MontoPago}, Saldo restante: {vale.MontoRestante}, Credito actualizado: {creditRequest.EstimatedCredit}.",
                CreatedAt = DateTime.UtcNow
            });

            return response;
        }

        public async Task<ServiceResponseDto<Vale>> ResolveByAdminAsync(string valeId, ResolveValeStatusDto dto, string? actorUser = null)
        {
            var response = new ServiceResponseDto<Vale>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? "admin" : actorUser;

            var normalizedStatus = dto.Status.Trim();
            if (!string.Equals(normalizedStatus, "Aceptado", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(normalizedStatus, "Rechazado", StringComparison.OrdinalIgnoreCase))
            {
                response.Status = false;
                response.Message = "Status invalido. Solo se permite 'Aceptado' o 'Rechazado'.";
                return response;
            }

            normalizedStatus = string.Equals(normalizedStatus, "Aceptado", StringComparison.OrdinalIgnoreCase)
                ? "Aceptado"
                : "Rechazado";

            var vale = await _valeRepository.GetByIdAsync(valeId);
            if (vale == null)
            {
                response.Status = false;
                response.Message = "Vale no encontrado.";
                return response;
            }

            if (!string.Equals(vale.Status, "Pendiente", StringComparison.OrdinalIgnoreCase))
            {
                response.Status = false;
                response.Message = "Solo se pueden resolver vales con status Pendiente.";
                return response;
            }

            if (normalizedStatus == "Rechazado")
            {
                var creditRequest = await _creditRequestRepository.GetActiveByUserIdAsync(vale.UserId);
                if (creditRequest == null)
                {
                    response.Status = false;
                    response.Message = "No se encontro un credito autorizado activo para devolver el saldo del vale rechazado.";
                    return response;
                }

                var refundAmount = vale.MontoRestante > 0 ? vale.MontoRestante : vale.MontoSolicitado;

                creditRequest.EstimatedCredit += refundAmount;
                await _creditRequestRepository.UpdateAsync(creditRequest);

                vale.MontoRestante = 0;
                vale.Status = "Rechazado";

                try
                {
                    await _valeRepository.UpdateAsync(vale);
                }
                catch
                {
                    creditRequest.EstimatedCredit -= refundAmount;
                    await _creditRequestRepository.UpdateAsync(creditRequest);
                    throw;
                }

                response.Data = vale;
                response.Message = $"Vale rechazado y saldo devuelto correctamente. Monto devuelto: {refundAmount}.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.ResolveByAdmin",
                    User = logUser,
                    Details = $"Vale rechazado. ValeId: {valeId}, UsuarioId: {vale.UserId}, Monto devuelto: {refundAmount}.",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            vale.Status = "Aceptado";
            await _valeRepository.UpdateAsync(vale);

            response.Data = vale;
            response.Message = "Vale aceptado correctamente.";

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "Vales.ResolveByAdmin",
                User = logUser,
                Details = $"Vale aceptado. ValeId: {valeId}, UsuarioId: {vale.UserId}.",
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

        public async Task<ServiceResponseDto<List<Vale>>> GetByUserAsync(int userId, string? status = null, string? actorUser = null)
        {
            var response = new ServiceResponseDto<List<Vale>>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? userId.ToString() : actorUser;

            try
            {
                response.Data = await _valeRepository.GetByUserIdAsync(userId, status);
                response.Message = "Vales del cliente obtenidos exitosamente.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.GetByUser",
                    User = logUser,
                    Details = $"Consulta de vales del cliente ejecutada. Filtro status: {status ?? "Todos"}. Total: {response.Data?.Count ?? 0}",
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error al obtener tus vales: " + ex.Message;

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Vales.GetByUser",
                    User = logUser,
                    Details = $"Error al consultar vales del cliente. Filtro status: {status ?? "Todos"}. Error: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return response;
        }
    }
}

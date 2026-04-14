using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Creditos;
using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Application.Interfaces.Auth;
using fenixjobs_api.Application.Interfaces.Creditos;
using fenixjobs_api.Domain.Documents;
using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Application.Services.Creditos
{
    public class CreditRequestService : ICreditRequestService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICreditRequestRepository _creditRequestRepository;
        private readonly ISystemLogRepository _logRepository;

        public CreditRequestService(IUserRepository userRepository, ICreditRequestRepository creditRequestRepository, ISystemLogRepository logRepository)
        {
            _userRepository = userRepository;
            _creditRequestRepository = creditRequestRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResponseDto<CreditRequestResponseDto>> CreateForUserAsync(int userId, CreateCreditRequestDto dto, string? actorUser = null)
        {
            var response = new ServiceResponseDto<CreditRequestResponseDto>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? userId.ToString() : actorUser;

            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.CurpRfc) ||
                string.IsNullOrWhiteSpace(dto.Address) ||
                string.IsNullOrWhiteSpace(dto.Phone))
            {
                response.Status = false;
                response.Message = "Todos los datos personales son obligatorios.";
                return response;
            }

            if (dto.MonthlyIncome <= 0)
            {
                response.Status = false;
                response.Message = "Los ingresos mensuales deben ser mayores a 0.";
                return response;
            }

            if (dto.References == null || dto.References.Count < 2)
            {
                response.Status = false;
                response.Message = "Debes enviar al menos 2 referencias.";
                return response;
            }

            if (dto.References.Any(reference =>
                    string.IsNullOrWhiteSpace(reference.Relationship) ||
                    string.IsNullOrWhiteSpace(reference.FullName) ||
                    string.IsNullOrWhiteSpace(reference.ContactPhone)))
            {
                response.Status = false;
                response.Message = "Cada referencia debe incluir parentesco, nombre y numero de contacto.";
                return response;
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Status = false;
                response.Message = "Usuario no encontrado.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "CreditRequests.Create",
                    User = logUser,
                    Details = "Solicitud de credito rechazada. Usuario no encontrado.",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            await Task.Delay(TimeSpan.FromMinutes(1));

            var estimatedCredit = CalculateEstimatedCredit(dto.MonthlyIncome, dto.References);

            var creditRequest = new CreditRequest
            {
                UserId = user.id_usuario,
                User = user,
                FullName = dto.FullName.Trim(),
                CurpRfc = dto.CurpRfc.Trim(),
                Address = dto.Address.Trim(),
                Phone = dto.Phone.Trim(),
                MonthlyIncome = dto.MonthlyIncome,
                EstimatedCredit = estimatedCredit,
                Status = "Estimado",
                CreatedAt = DateTime.UtcNow,
                References = dto.References.Select(reference => new CreditReference
                {
                    Relationship = reference.Relationship.Trim(),
                    FullName = reference.FullName.Trim(),
                    ContactPhone = reference.ContactPhone.Trim()
                }).ToList()
            };

            await _creditRequestRepository.AddAsync(creditRequest);

            response.Data = new CreditRequestResponseDto
            {
                Id = creditRequest.Id,
                UserId = creditRequest.UserId,
                UserName = user.usuario,
                FullName = creditRequest.FullName,
                CurpRfc = creditRequest.CurpRfc,
                Address = creditRequest.Address,
                Phone = creditRequest.Phone,
                MonthlyIncome = creditRequest.MonthlyIncome,
                EstimatedCredit = creditRequest.EstimatedCredit,
                Status = creditRequest.Status,
                CreatedAt = creditRequest.CreatedAt,
                References = creditRequest.References.Select(reference => new CreditReferenceDto
                {
                    Relationship = reference.Relationship,
                    FullName = reference.FullName,
                    ContactPhone = reference.ContactPhone
                }).ToList()
            };
            response.Message = "Solicitud de credito registrada y estimada exitosamente.";

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "CreditRequests.Create",
                User = logUser,
                Details = $"Solicitud de credito creada. Ingresos: {dto.MonthlyIncome}, Credito estimado: {estimatedCredit}, Referencias: {dto.References.Count}",
                CreatedAt = DateTime.UtcNow
            });

            return response;
        }

        public async Task<ServiceResponseDto<CreditBalanceDto>> GetBalanceAsync(int userId, string? actorUser = null)
        {
            var response = new ServiceResponseDto<CreditBalanceDto>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? userId.ToString() : actorUser;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Status = false;
                response.Message = "Usuario no encontrado.";
                return response;
            }

            var creditRequest = await _creditRequestRepository.GetActiveByUserIdAsync(userId);
            if (creditRequest == null)
            {
                response.Status = false;
                response.Message = "No cuentas con un credito autorizado activo.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "CreditRequests.GetBalance",
                    User = logUser,
                    Details = "Consulta de saldo rechazada. Sin credito activo.",
                    CreatedAt = DateTime.UtcNow
                });

                return response;
            }

            response.Data = new CreditBalanceDto
            {
                CreditRequestId = creditRequest.Id,
                UserId = creditRequest.UserId,
                UserName = user.usuario,
                SaldoDisponible = creditRequest.EstimatedCredit,
                Status = creditRequest.Status,
                CreatedAt = creditRequest.CreatedAt
            };
            response.Message = "Saldo de credito obtenido exitosamente.";

            await _logRepository.AddLogAsync(new SystemLog
            {
                Action = "CreditRequests.GetBalance",
                User = logUser,
                Details = $"Saldo consultado exitosamente. Credito disponible: {creditRequest.EstimatedCredit}.",
                CreatedAt = DateTime.UtcNow
            });

            return response;
        }

        public async Task<ServiceResponseDto<List<ClientCreditSummaryDto>>> GetClientsWithCreditAsync(string? actorUser = null)
        {
            var response = new ServiceResponseDto<List<ClientCreditSummaryDto>>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? "admin" : actorUser;

            try
            {
                response.Data = await _creditRequestRepository.GetClientsWithCreditAsync();
                response.Message = "Clientes con credito solicitado obtenidos exitosamente.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "CreditRequests.GetClientsWithCredit",
                    User = logUser,
                    Details = $"Consulta de clientes con credito ejecutada. Total: {response.Data?.Count ?? 0}",
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error al obtener clientes con credito: " + ex.Message;

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "CreditRequests.GetClientsWithCredit",
                    User = logUser,
                    Details = $"Error al consultar clientes con credito. Error: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return response;
        }

        private static decimal CalculateEstimatedCredit(decimal monthlyIncome, IReadOnlyCollection<CreditReferenceDto> references)
        {
            var incomeBonus = monthlyIncome switch
            {
                < 5000 => 0.25m,
                < 10000 => 0.5m,
                < 20000 => 0.75m,
                _ => 1.0m
            };

            var referencesBonus = references.Sum(reference => GetReferenceBonus(reference.Relationship));
            var multiplier = 3.5m + incomeBonus + referencesBonus;

            return Math.Round(monthlyIncome * multiplier, 2);
        }

        private static decimal GetReferenceBonus(string relationship)
        {
            var normalizedRelationship = relationship.Trim().ToLowerInvariant();

            if (normalizedRelationship.Contains("padre") ||
                normalizedRelationship.Contains("madre") ||
                normalizedRelationship.Contains("espos") ||
                normalizedRelationship.Contains("conyug") ||
                normalizedRelationship.Contains("herman") ||
                normalizedRelationship.Contains("jefe"))
            {
                return 0.25m;
            }

            if (normalizedRelationship.Contains("amig") ||
                normalizedRelationship.Contains("vecin") ||
                normalizedRelationship.Contains("compan") ||
                normalizedRelationship.Contains("coleg"))
            {
                return 0.15m;
            }

            return 0.1m;
        }
    }
}
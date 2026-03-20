using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Customers;
using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Application.Interfaces.Customers;
using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISystemLogRepository _logRepository;

        public CustomerService(ICustomerRepository customerRepository, ISystemLogRepository logRepository)
        {
            _customerRepository = customerRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResponseDto<List<CustomerDto>>> GetAllAsync(string? type = null, string? actorUser = null)
        {
            var response = new ServiceResponseDto<List<CustomerDto>>();
            var logUser = string.IsNullOrWhiteSpace(actorUser) ? "unknown" : actorUser;

            try
            {
                response.Data = await _customerRepository.GetAllAsync(type);

                response.Message = "Customers obtenidos exitosamente.";

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Customers.GetAll",
                    User = logUser,
                    Details = $"Consulta de customers ejecutada. Filtro type: {type ?? "Todos"}. Total: {response.Data?.Count ?? 0}",
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error al obtener customers: " + ex.Message;

                await _logRepository.AddLogAsync(new SystemLog
                {
                    Action = "Customers.GetAll",
                    User = logUser,
                    Details = $"Error al consultar customers. Filtro type: {type ?? "Todos"}. Error: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return response;
        }
    }
}

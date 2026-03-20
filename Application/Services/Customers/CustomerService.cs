using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Customers;
using fenixjobs_api.Application.Interfaces.Customers;

namespace fenixjobs_api.Application.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResponseDto<List<CustomerDto>>> GetAllAsync(string? type = null)
        {
            var response = new ServiceResponseDto<List<CustomerDto>>();

            try
            {
                response.Data = await _customerRepository.GetAllAsync(type);

                response.Message = "Customers obtenidos exitosamente.";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Error al obtener customers: " + ex.Message;
            }

            return response;
        }
    }
}

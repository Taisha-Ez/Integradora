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

        public async Task<ServiceResponseDto<List<CustomerDto>>> GetAllAsync()
        {
            var response = new ServiceResponseDto<List<CustomerDto>>();

            try
            {
                var customers = await _customerRepository.GetAllAsync();

                response.Data = customers.Select(c => new CustomerDto
                {
                    Id = c.id,
                    Type = c.Type,
                    IdUser = c.id_user,
                    UserName = c.User?.usuario ?? string.Empty
                }).ToList();

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

using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Customers;

namespace fenixjobs_api.Application.Interfaces.Customers
{
    public interface ICustomerService
    {
        Task<ServiceResponseDto<List<CustomerDto>>> GetAllAsync(string? type = null);
    }
}

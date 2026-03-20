using fenixjobs_api.Application.DTOs.Customers;

namespace fenixjobs_api.Application.Interfaces.Customers
{
    public interface ICustomerRepository
    {
        Task<List<CustomerDto>> GetAllAsync(string? type = null);
    }
}

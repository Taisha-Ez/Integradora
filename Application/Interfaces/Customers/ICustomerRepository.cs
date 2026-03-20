using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Application.Interfaces.Customers
{
    public interface ICustomerRepository
    {
        Task<List<TypeCustomers>> GetAllAsync();
    }
}

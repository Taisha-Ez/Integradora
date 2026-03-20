using fenixjobs_api.Application.Interfaces.Customers;
using fenixjobs_api.Application.DTOs.Customers;
using fenixjobs_api.Infrastructure.Persistence.MySQL;
using Microsoft.EntityFrameworkCore;

namespace fenixjobs_api.Infrastructure.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly FenixDbContext _context;

        public CustomerRepository(FenixDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            return await _context.TypeCustomers
                .AsNoTracking()
                .Join(
                    _context.Users.AsNoTracking(),
                    customer => customer.id_user,
                    user => user.id_usuario,
                    (customer, user) => new CustomerDto
                    {
                        Id = customer.id,
                        Type = customer.Type,
                        IdUser = customer.id_user,
                        UserName = user.usuario
                    })
                .ToListAsync();
        }
    }
}

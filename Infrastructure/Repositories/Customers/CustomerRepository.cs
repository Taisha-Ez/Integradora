using fenixjobs_api.Application.Interfaces.Customers;
using fenixjobs_api.Domain.Entities;
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

        public async Task<List<TypeCustomers>> GetAllAsync()
        {
            return await _context.TypeCustomers
                .AsNoTracking()
                .Include(c => c.User)
                .ToListAsync();
        }
    }
}

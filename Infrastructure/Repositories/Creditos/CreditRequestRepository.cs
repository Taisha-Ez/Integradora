using fenixjobs_api.Application.Interfaces.Creditos;
using fenixjobs_api.Domain.Entities;
using fenixjobs_api.Infrastructure.Persistence.MySQL;

namespace fenixjobs_api.Infrastructure.Repositories.Creditos
{
    public class CreditRequestRepository : ICreditRequestRepository
    {
        private readonly FenixDbContext _context;

        public CreditRequestRepository(FenixDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CreditRequest creditRequest)
        {
            await _context.CreditRequests.AddAsync(creditRequest);
            await _context.SaveChangesAsync();
        }
    }
}
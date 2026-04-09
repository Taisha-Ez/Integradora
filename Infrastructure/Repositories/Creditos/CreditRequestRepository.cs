using fenixjobs_api.Application.Interfaces.Creditos;
using fenixjobs_api.Domain.Entities;
using fenixjobs_api.Infrastructure.Persistence.MySQL;
using Microsoft.EntityFrameworkCore;

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

        public async Task<CreditRequest?> GetActiveByUserIdAsync(int userId)
        {
            return await _context.CreditRequests
                .Where(request =>
                    request.UserId == userId &&
                    request.EstimatedCredit > 0 &&
                    (request.Status == "Estimado" || request.Status == "Activo" || request.Status == "Aprobado"))
                .OrderByDescending(request => request.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(CreditRequest creditRequest)
        {
            _context.CreditRequests.Update(creditRequest);
            await _context.SaveChangesAsync();
        }
    }
}
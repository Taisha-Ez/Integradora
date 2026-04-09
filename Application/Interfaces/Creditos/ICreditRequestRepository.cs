using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Application.Interfaces.Creditos
{
    public interface ICreditRequestRepository
    {
        Task AddAsync(CreditRequest creditRequest);
        Task<CreditRequest?> GetActiveByUserIdAsync(int userId);
        Task UpdateAsync(CreditRequest creditRequest);
    }
}
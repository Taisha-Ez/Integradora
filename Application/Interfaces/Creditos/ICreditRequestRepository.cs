using fenixjobs_api.Domain.Entities;
using fenixjobs_api.Application.DTOs.Creditos;

namespace fenixjobs_api.Application.Interfaces.Creditos
{
    public interface ICreditRequestRepository
    {
        Task AddAsync(CreditRequest creditRequest);
        Task<CreditRequest?> GetActiveByUserIdAsync(int userId);
        Task UpdateAsync(CreditRequest creditRequest);
        Task<List<ClientCreditSummaryDto>> GetClientsWithCreditAsync();
    }
}
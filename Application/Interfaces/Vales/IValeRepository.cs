using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Interfaces.Vales
{
    public interface IValeRepository
    {
        Task CreateAsync(Vale vale);
        Task<Vale?> GetByIdAsync(string valeId);
        Task UpdateAsync(Vale vale);
        Task<List<Vale>> GetAllAsync(string? status = null);
        Task<List<Vale>> GetByUserIdAsync(int userId, string? status = null);
    }
}

using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetByUsuarioAsync(string usuario); // Cambiado
        Task AddAsync(Users user);
    }
}
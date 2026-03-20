using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Application.Interfaces.Auth
{
    public interface IUserRepository
    {
        Task<Users?> GetByUsuarioAsync(string usuario);
        Task AddAsync(Users user);
    }
}

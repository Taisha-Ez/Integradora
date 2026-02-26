using fenixjobs_api.Application.DTOs;
using fenixjobs_api.Domain.Entities;
using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponseDto<Users>> RegisterAsync(RegisterDto dto);

        Task<ServiceResponseDto<string>> LoginAsync(LoginDto dto);
    }
}

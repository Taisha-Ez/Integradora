using fenixjobs_api.Application.DTOs.Auth;
using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ServiceResponseDto<Users>> RegisterAsync(RegisterDto dto);

        Task<ServiceResponseDto<string>> LoginAsync(LoginDto dto);
    }
}

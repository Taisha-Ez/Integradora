using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Creditos;

namespace fenixjobs_api.Application.Interfaces.Creditos
{
    public interface ICreditRequestService
    {
        Task<ServiceResponseDto<CreditRequestResponseDto>> CreateForUserAsync(int userId, CreateCreditRequestDto dto, string? actorUser = null);
    }
}
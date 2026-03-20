using fenixjobs_api.Application.DTOs.Common;
using fenixjobs_api.Application.DTOs.Vales;
using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Interfaces.Vales
{
    public interface IValeService
    {
        Task<ServiceResponseDto<Vale>> CreateForClientAsync(int userId, CreateValeDto dto, string? actorUser = null);
        Task<ServiceResponseDto<List<Vale>>> GetAllAsync(string? status = null, string? actorUser = null);
    }
}

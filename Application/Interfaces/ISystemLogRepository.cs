using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Interfaces
{
    public interface ISystemLogRepository
    {
        Task AddLogAsync(SystemLog log);
    }
}

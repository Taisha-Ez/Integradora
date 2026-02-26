using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Domain.Documents;
using fenixjobs_api.Infrastructure.Persistence.MongoDB;

namespace fenixjobs_api.Infrastructure.Repositories
{
    public class SystemLogRepository : ISystemLogRepository
    {
        private readonly FenixMongoContext _context;

        public SystemLogRepository(FenixMongoContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(SystemLog log)
        {
            await _context.SystemLogs.InsertOneAsync(log);
        }
    }
}

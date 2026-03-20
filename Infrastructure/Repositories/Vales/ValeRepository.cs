using fenixjobs_api.Application.Interfaces.Vales;
using fenixjobs_api.Domain.Documents;
using fenixjobs_api.Infrastructure.Persistence.MongoDB;

namespace fenixjobs_api.Infrastructure.Repositories.Vales
{
    public class ValeRepository : IValeRepository
    {
        private readonly FenixMongoContext _context;

        public ValeRepository(FenixMongoContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Vale vale)
        {
            await _context.Vales.InsertOneAsync(vale);
        }
    }
}

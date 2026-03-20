using fenixjobs_api.Application.Interfaces.Vales;
using fenixjobs_api.Domain.Documents;
using fenixjobs_api.Infrastructure.Persistence.MongoDB;
using MongoDB.Driver;

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

        public async Task<List<Vale>> GetAllAsync(string? status = null)
        {
            var filter = string.IsNullOrWhiteSpace(status)
                ? Builders<Vale>.Filter.Empty
                : Builders<Vale>.Filter.Eq(v => v.Status, status);

            return await _context.Vales
                .Find(filter)
                .SortByDescending(v => v.CreatedAt)
                .ToListAsync();
        }
    }
}

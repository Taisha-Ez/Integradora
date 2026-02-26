using fenixjobs_api.Domain.Documents;
using MongoDB.Driver;

namespace fenixjobs_api.Infrastructure.Persistence.MongoDB
{
    public class FenixMongoContext
    {
        private readonly IMongoDatabase _database;

        public FenixMongoContext(IMongoDatabase database)
        {
            _database = database;
        }

        public IMongoCollection<SystemLog> SystemLogs => _database.GetCollection<SystemLog>("SystemLogs");
    }
}

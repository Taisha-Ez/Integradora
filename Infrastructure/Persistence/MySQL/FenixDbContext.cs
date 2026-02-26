using Microsoft.EntityFrameworkCore;
using fenixjobs_api.Domain.Entities;

namespace fenixjobs_api.Infrastructure.Persistence.MySQL
{
    public class FenixDbContext : DbContext
    {
        public FenixDbContext(DbContextOptions<FenixDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
    }
}

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
        public DbSet<TypeCustomers> TypeCustomers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.usuario)
                .IsUnique();

            modelBuilder.Entity<TypeCustomers>()
                .HasOne(tc => tc.User)
                .WithMany(u => u.TypeCustomers)
                .HasForeignKey(tc => tc.id_user)
                .HasPrincipalKey(u => u.id_usuario);
        }
    }
}

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
        public DbSet<CreditRequest> CreditRequests { get; set; }
        public DbSet<CreditReference> CreditReferences { get; set; }

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

            modelBuilder.Entity<CreditRequest>()
                .HasOne(cr => cr.User)
                .WithMany()
                .HasForeignKey(cr => cr.UserId)
                .HasPrincipalKey(u => u.id_usuario);

            modelBuilder.Entity<CreditReference>()
                .HasOne(cr => cr.CreditRequest)
                .WithMany(request => request.References)
                .HasForeignKey(cr => cr.CreditRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CreditRequest>()
                .Property(request => request.EstimatedCredit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<CreditRequest>()
                .Property(request => request.MonthlyIncome)
                .HasPrecision(18, 2);
        }
    }
}

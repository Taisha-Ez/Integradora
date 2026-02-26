using fenixjobs_api.Application.Interfaces;
using fenixjobs_api.Domain.Entities;
using fenixjobs_api.Infrastructure.Persistence.MySQL;
using Microsoft.EntityFrameworkCore;

namespace fenixjobs_api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FenixDbContext _context;

        public UserRepository(FenixDbContext context)
        {
            _context = context;
        }

        public async Task<Users?> GetByUsuarioAsync(string usuario)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.usuario == usuario);
        }

        public async Task<Users?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(Users employee)
        {
            await _context.Users.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Users employee)
        {
            _context.Users.Update(employee);
            await _context.SaveChangesAsync();
        }
    }
}

using fenixjobs_api.Application.DTOs.Creditos;
using fenixjobs_api.Application.Interfaces.Creditos;
using fenixjobs_api.Domain.Entities;
using fenixjobs_api.Infrastructure.Persistence.MySQL;
using Microsoft.EntityFrameworkCore;

namespace fenixjobs_api.Infrastructure.Repositories.Creditos
{
    public class CreditRequestRepository : ICreditRequestRepository
    {
        private readonly FenixDbContext _context;

        public CreditRequestRepository(FenixDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CreditRequest creditRequest)
        {
            await _context.CreditRequests.AddAsync(creditRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<CreditRequest?> GetActiveByUserIdAsync(int userId)
        {
            return await _context.CreditRequests
                .Where(request =>
                    request.UserId == userId &&
                    request.EstimatedCredit > 0 &&
                    (request.Status == "Estimado" || request.Status == "Activo" || request.Status == "Aprobado"))
                .OrderByDescending(request => request.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(CreditRequest creditRequest)
        {
            _context.CreditRequests.Update(creditRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ClientCreditSummaryDto>> GetClientsWithCreditAsync()
        {
            var creditRows = await (
                from creditRequest in _context.CreditRequests.AsNoTracking()
                join user in _context.Users.AsNoTracking()
                    on creditRequest.UserId equals user.id_usuario into userGroup
                from user in userGroup.DefaultIfEmpty()
                where user == null || user.tipo_usuario == "cliente"
                orderby creditRequest.CreatedAt descending
                select new
                {
                    UserId = creditRequest.UserId,
                    Usuario = user != null ? user.usuario : string.Empty,
                    Nombre = user != null ? user.nombre : creditRequest.FullName,
                    ApellidoPaterno = user != null ? user.apellido_paterno : null,
                    ApellidoMaterno = user != null ? user.apellido_materno : null,
                    TipoUsuario = user != null ? user.tipo_usuario : "cliente",
                    CreditRequestId = creditRequest.Id,
                    CurpRfc = creditRequest.CurpRfc,
                    MonthlyIncome = creditRequest.MonthlyIncome,
                    EstimatedCredit = creditRequest.EstimatedCredit,
                    Status = creditRequest.Status,
                    CreatedAt = creditRequest.CreatedAt
                })
                .ToListAsync();

            return creditRows
                .GroupBy(item => item.UserId)
                .Select(group =>
                {
                    var latest = group.First();

                    return new ClientCreditSummaryDto
                    {
                        UserId = latest.UserId,
                        Usuario = latest.Usuario,
                        NombreCompleto = string.Join(" ", new[] { latest.Nombre, latest.ApellidoPaterno, latest.ApellidoMaterno }.Where(part => !string.IsNullOrWhiteSpace(part))),
                        TipoUsuario = latest.TipoUsuario,
                        CreditRequestId = latest.CreditRequestId,
                        CurpRfc = latest.CurpRfc,
                        MonthlyIncome = latest.MonthlyIncome,
                        EstimatedCredit = latest.EstimatedCredit,
                        Status = latest.Status,
                        CreatedAt = latest.CreatedAt
                    };
                })
                .OrderByDescending(item => item.CreatedAt)
                .ToList();
        }
    }
}
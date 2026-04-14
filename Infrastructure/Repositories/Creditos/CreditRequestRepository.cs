using fenixjobs_api.Application.Interfaces.Creditos;
using fenixjobs_api.Application.DTOs.Creditos;
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
            var creditRows = await _context.CreditRequests
                .AsNoTracking()
                .Join(
                    _context.Users.AsNoTracking(),
                    creditRequest => creditRequest.UserId,
                    user => user.id_usuario,
                    (creditRequest, user) => new
                    {
                        UserId = user.id_usuario,
                        Usuario = user.usuario,
                        Nombre = user.nombre,
                        ApellidoPaterno = user.apellido_paterno,
                        ApellidoMaterno = user.apellido_materno,
                        TipoUsuario = user.tipo_usuario,
                        CreditRequestId = creditRequest.Id,
                        CurpRfc = creditRequest.CurpRfc,
                        MonthlyIncome = creditRequest.MonthlyIncome,
                        EstimatedCredit = creditRequest.EstimatedCredit,
                        Status = creditRequest.Status,
                        CreatedAt = creditRequest.CreatedAt
                    })
                .Where(item => item.TipoUsuario == "cliente")
                .OrderByDescending(item => item.CreatedAt)
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
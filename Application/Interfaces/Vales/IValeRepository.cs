using fenixjobs_api.Domain.Documents;

namespace fenixjobs_api.Application.Interfaces.Vales
{
    public interface IValeRepository
    {
        Task CreateAsync(Vale vale);
    }
}

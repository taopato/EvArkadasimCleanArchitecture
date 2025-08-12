using System.Linq;
using Domain.Entities;

namespace Application.Services.Repositories
{
    public interface ILedgerReadRepository
    {
        IQueryable<LedgerEntry> Query();
    }
}

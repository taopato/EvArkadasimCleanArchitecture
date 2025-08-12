using System.Linq;
using Domain.Entities;

namespace Application.Services.Repositories
{
    public interface IBillReadRepository
    {
        IQueryable<Bill> Query();
    }
}

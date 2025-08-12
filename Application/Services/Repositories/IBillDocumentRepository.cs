using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Entities;

namespace Application.Services.Repositories
{
    public interface IBillDocumentRepository
    {
        Task<BillDocument> AddAsync(BillDocument doc);
        Task<List<BillDocument>> GetByBillAsync(int billId);
        Task DeleteAsync(int id);
    }
}

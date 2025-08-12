using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Repositories
{
    public interface IUtilityBillRepository
    {
        Task<Bill> AddAsync(Bill bill);
        Task<Bill?> GetAsync(int id);
        Task<Bill?> GetByHouseMonthAsync(int houseId, UtilityType type, string month);
        Task<List<Bill>> GetRecentAsync(int houseId, UtilityType? type, int limit);
        Task UpdateAsync(Bill bill);
    }
}

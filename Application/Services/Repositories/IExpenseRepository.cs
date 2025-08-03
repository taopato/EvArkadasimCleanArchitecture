using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense> AddAsync(Expense expense);
        Task<List<Expense>> GetAllAsync();
        Task<int> GetHouseMemberCountAsync(int kaydedenUserId);
    }
}

using Application.Services.Repositories;
using Domain.Entities;
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _context;

        public EfExpenseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Expense> AddAsync(Expense expense)
        {
            var added = await _context.Set<Expense>().AddAsync(expense);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<List<Expense>> GetAllAsync()
        {
            return await _context.Expenses
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<int> GetHouseMemberCountAsync(int kaydedenUserId)
        {
            // Örneğin: Kaydeden kullanıcının ev grubundaki üye sayısı
            return await _context.HouseMembers
                                 .CountAsync(hm => hm.UserId == kaydedenUserId);
        }
    }
}

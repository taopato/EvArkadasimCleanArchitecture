// Persistence/Repositories/EfPaymentRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfPaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public EfPaymentRepository(AppDbContext context) => _context = context;

        public async Task<List<Payment>> GetAllAsync()
            => await _context.Payments.ToListAsync();

        public async Task<decimal> GetTotalAlacaklıAsync(int houseId, int userId)
            // houseId’i şimdilik kullanmıyoruz
            => await _context.Payments
                .Where(p => p.AlacakliUserId == userId)
                .SumAsync(p => (decimal?)p.Tutar) ?? 0m;

        public async Task<decimal> GetTotalBorçluAsync(int houseId, int userId)
            => await _context.Payments
                .Where(p => p.BorcluUserId == userId)
                .SumAsync(p => (decimal?)p.Tutar) ?? 0m;

        public async Task<Payment> AddAsync(Payment entity)
        {
            var added = await _context.Payments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<List<Payment>> GetByHouseIdAsync(int houseId)
        {
            return await _context.Payments
                                 .Where(p => p.HouseId == houseId)
                                 .ToListAsync();
        }
    }
}

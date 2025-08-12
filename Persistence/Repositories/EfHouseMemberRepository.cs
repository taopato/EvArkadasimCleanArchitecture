using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfHouseMemberRepository : IHouseMemberRepository
    {
        private readonly AppDbContext _context;

        public EfHouseMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<HouseMember> Query()
        {
            return _context.HouseMembers.AsQueryable();
        }

        public async Task<List<HouseMember>> GetByHouseIdAsync(int houseId)
        {
            return await _context.HouseMembers
                .Where(h => h.HouseId == houseId)
                .Include(h => h.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<House> GetByIdWithMembersAsync(int houseId)
        {
            return await _context.Houses
                .Include(h => h.HouseMembers)
                    .ThenInclude(hm => hm.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == houseId);
        }

        // GEÇİCİ: IsActive yoksa tüm üyeleri alıyoruz.
        // Entity'ni gönderdiğinde burada gerçek "aktif" şartını uygularız.
        public async Task<List<int>> GetActiveUserIdsAsync(int houseId, CancellationToken ct = default)
        {
            return await _context.HouseMembers
                .AsNoTracking()                     // <-- ÖNCE AsNoTracking
                .Where(hm => hm.HouseId == houseId) // TODO: aktif filtre ekleyeceğiz
                .Select(hm => hm.UserId)            // <-- Sonra projection
                .ToListAsync(ct);
        }
    }
}

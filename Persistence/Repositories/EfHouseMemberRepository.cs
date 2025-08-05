// Persistence/Repositories/EfHouseMemberRepository.cs
using Application.Services.Repositories;
using Domain.Entities;
using Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class EfHouseMemberRepository : IHouseMemberRepository
    {
        private readonly AppDbContext _context;
        public EfHouseMemberRepository(AppDbContext context) => _context = context;

        public async Task<List<HouseMember>> GetByHouseIdAsync(int houseId) =>
            await _context.HouseMembers
                          .Where(hm => hm.HouseId == houseId)
                          .ToListAsync();
        public IQueryable<HouseMember> Query()
        {
            return _context.HouseMembers.AsQueryable();
        }

    }
}

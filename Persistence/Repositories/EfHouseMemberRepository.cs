using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Application.Services.Repositories;

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
                .ToListAsync();
        }

        public Task<House> GetByIdWithMembersAsync(int houseId)
        {
            throw new NotImplementedException();
        }
    }
}

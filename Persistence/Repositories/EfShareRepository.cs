using Application.Services.Repositories;
using Domain.Entities;
using Persistence.Contexts;
using System.Threading.Tasks;


namespace Persistence.Repositories
{
    public class EfShareRepository : IShareRepository
    {
        private readonly AppDbContext _context;
        public EfShareRepository(AppDbContext context)
            => _context = context;

        public async Task<Share> AddAsync(Share entity)
        {
            var added = await _context.Shares.AddAsync(entity);
            await _context.SaveChangesAsync();
            return added.Entity;
        }
        public async Task DeleteAsync(Share entity)
        {
            _context.Shares.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

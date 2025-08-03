using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Services.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public EfUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> AddAsync(User user) // ✅ Dönüş tipi düzeltildi
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user; // ✅ Eklenen kullanıcıyı geri döndürüyoruz
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

// Persistence/Repositories/EfVerificationCodeRepository.cs
using Application.Services.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class EfVerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly AppDbContext _context;
        public EfVerificationCodeRepository(AppDbContext context) => _context = context;

        public async Task<VerificationCode> GetByEmailAsync(string email)
            => await _context.VerificationCodes
                .FirstOrDefaultAsync(vc => vc.Email == email);

        public async Task AddOrUpdateAsync(VerificationCode entity)
        {
            var existing = await GetByEmailAsync(entity.Email);
            if (existing == null)
                await _context.VerificationCodes.AddAsync(entity);
            else
            {
                existing.Code = entity.Code;
                existing.CreatedAt = entity.CreatedAt;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(VerificationCode entity)
        {
            _context.VerificationCodes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

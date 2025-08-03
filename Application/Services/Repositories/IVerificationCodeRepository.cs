// Application/Services/Repositories/IVerificationCodeRepository.cs
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Services.Repositories
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode> GetByEmailAsync(string email);
        Task AddOrUpdateAsync(VerificationCode entity);
        Task DeleteAsync(VerificationCode entity);
    }
}

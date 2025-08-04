using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();
        Task<User> AddAsync(User user); // <== dönüş türü Task<User> olmalı
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<User?> GetByEmailAsync(string email);



    }
}

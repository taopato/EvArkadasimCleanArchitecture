// Application/Services/Repositories/IShareRepository.cs
using Domain.Entities;

public interface IShareRepository
{
    Task<Share> AddAsync(Share entity);
    Task DeleteAsync(Share entity);  // eklendi

}

// Application/Services/Repositories/IHouseMemberRepository.cs
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Repositories
{
    public interface IHouseMemberRepository
    {
        Task<List<HouseMember>> GetByHouseIdAsync(int houseId);
    }
}

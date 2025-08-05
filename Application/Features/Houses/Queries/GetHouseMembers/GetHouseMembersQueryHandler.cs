using Application.Features.Houses.Dtos;
using Application.Services.Repositories;
using Core.Utilities.Results;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore; // 👈 Include() için gerekli
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Houses.Queries.GetHouseMembers
{
    public class GetHouseMembersQueryHandler
        : IRequestHandler<GetHouseMembersQuery, Response<List<HouseMemberDto>>>
    {
        private readonly IHouseMemberRepository _houseMemberRepository;

        public GetHouseMembersQueryHandler(IHouseMemberRepository houseMemberRepository)
        {
            _houseMemberRepository = houseMemberRepository;
        }

        public async Task<Response<List<HouseMemberDto>>> Handle(GetHouseMembersQuery request, CancellationToken cancellationToken)
        {
            var members = await _houseMemberRepository
                .Query()
                .Include(x => x.User)
                .Where(x => x.HouseId == request.HouseId)
                .Select(x => new HouseMemberDto
                {
                    Id = x.UserId,
                    FullName = x.User.FirstName + " " + x.User.LastName,
                    Email = x.User.Email
                })
                .ToListAsync(cancellationToken); // 🧠 cancellationToken da eklendi

            return new Response<List<HouseMemberDto>>(members, true);
        }
    }
}

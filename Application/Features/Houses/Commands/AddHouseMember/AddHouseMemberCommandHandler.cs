using MediatR;
using Application.Services.Repositories;
using Domain.Entities;

namespace Application.Features.Houses.Commands.AddHouseMember
{
    public class AddHouseMemberCommandHandler
        : IRequestHandler<AddHouseMemberCommand>
    {
        private readonly IHouseRepository _houseRepository;

        public AddHouseMemberCommandHandler(IHouseRepository houseRepository)
        {
            _houseRepository = houseRepository;
        }

        public async Task<Unit> Handle(AddHouseMemberCommand request, CancellationToken cancellationToken)
        {
            var member = new HouseMember
            {
                HouseId = request.HouseId,
                UserId = request.UserId
            };
            await _houseRepository.AddMemberAsync(member);
            return Unit.Value;
        }
    }
}

using Application.Features.Houses.Dtos;
using MediatR;

namespace Application.Features.Houses.Commands.AddHouseMember
{
    public class AddHouseMemberCommand : IRequest
    {
        public int HouseId { get; set; }
        public int UserId { get; set; }
    }
}

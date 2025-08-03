using Application.Features.Users.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Users.Queries.GetUserList
{
    public class GetUserListQuery : IRequest<List<UserDto>>
    {
    }
}

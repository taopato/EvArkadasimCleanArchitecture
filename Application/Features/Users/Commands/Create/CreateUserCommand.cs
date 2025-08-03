using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.Create
{
    public class CreateUserCommand : IRequest<CreatedUserDto>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}

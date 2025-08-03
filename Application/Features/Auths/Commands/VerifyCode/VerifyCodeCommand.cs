// Application/Features/Auths/Commands/VerifyCode/VerifyCodeCommand.cs
using Application.Features.Auths.Dtos;
using MediatR;

namespace Application.Features.Auths.Commands.VerifyCode
{
    public class VerifyCodeCommand : IRequest<AuthResultDto>
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

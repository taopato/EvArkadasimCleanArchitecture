// Application/Features/Auths/Commands/SendVerificationCode/SendVerificationCodeCommand.cs
using MediatR;

namespace Application.Features.Auths.Commands.SendVerificationCode
{
    public class SendVerificationCodeCommand : IRequest
    {
        public string Email { get; set; }
    }
}

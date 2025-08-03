// Application/Features/Auths/Commands/SendVerificationCode/SendVerificationCodeCommandHandler.cs
using Core.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auths.Commands.SendVerificationCode
{
    public class SendVerificationCodeCommandHandler : IRequestHandler<SendVerificationCodeCommand>
    {
        private readonly IMailService _mailService;

        public SendVerificationCodeCommandHandler(IMailService mailService)
        {
            _mailService = mailService;
        }

        public Task<Unit> Handle(SendVerificationCodeCommand request, CancellationToken cancellationToken)
        {
            // 6 haneli rastgele kod
            var code = new Random().Next(100000, 999999).ToString();
            _mailService.SendVerificationCode(request.Email, code);
            return Task.FromResult(Unit.Value);
        }
    }
}

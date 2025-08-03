// Application/Features/Auths/Commands/VerifyCode/VerifyCodeCommandHandler.cs
using Application.Features.Auths.Dtos;
using Application.Services.Repositories;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Application.Features.Auths.Commands.VerifyCode
{
    public class VerifyCodeCommandHandler : IRequestHandler<VerifyCodeCommand, AuthResultDto>
    {
        private readonly IVerificationCodeRepository _codeRepo;
        private readonly IUserRepository             _userRepo;
        private readonly ITokenHelper                _tokenHelper;

        public VerifyCodeCommandHandler(
            IVerificationCodeRepository codeRepo,
            IUserRepository userRepo,
            ITokenHelper tokenHelper)
        {
            _codeRepo     = codeRepo;
            _userRepo     = userRepo;
            _tokenHelper  = tokenHelper;
        }

        public async Task<AuthResultDto> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
        {
            // 1. Kod kontrolü
            var verification = await _codeRepo.GetByEmailAsync(request.Email);
            if (verification == null
                || verification.Code != request.Code
                || (DateTime.UtcNow - verification.CreatedAt).TotalMinutes > 5)
            {
                throw new Exception("Geçersiz veya süresi dolmuş kod.");
            }

            // 2. Kullanıcıyı ekle (Domain.Entities.User içindeki alanlara uygun)
            var user = new User
            {
                FirstName        = request.FirstName,
                LastName         = request.LastName,
                Email            = request.Email,
                PasswordHash     = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt        = DateTime.UtcNow,
                RegistrationDate = DateTime.UtcNow
            };
            await _userRepo.AddAsync(user);

            // 3. Kod kaydını sil
            await _codeRepo.DeleteAsync(verification);

            // 4. Token oluştur
            var token = _tokenHelper.CreateToken(user);

            return new AuthResultDto
            {
                Token      = token.Token,
                Expiration = token.Expiration
            };
        }
    }
}

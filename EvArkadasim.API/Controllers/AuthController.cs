// EvArkadasim.API/Controllers/AuthController.cs
using Application.Features.Auths.Commands.Login;
using Application.Features.Auths.Commands.ResetPassword;
using Application.Features.Auths.Commands.SendVerificationCode;
using Application.Features.Auths.Commands.VerifyCodeAndRegister;
using Application.Features.Auths.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EvArkadasim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        // A) Sadece kod doğrulama (reset akışı için)
        [HttpPost("VerifyCodeForReset")]
        public async Task<IActionResult> VerifyCodeForReset(
            [FromBody] VerifyCodeRequestDto dto)
        {
            var cmd = new VerifyCodeAndRegisterCommand
            {
                Email = dto.Email,
                Code = dto.Code,
                FullName = null!,     // handler’da fullname==null ise bu kod akışına girer
                Password = string.Empty
            };
            var res = await _mediator.Send(cmd);
            return Ok(res);
        }

        // B) Kayıt veya kodla kayıt akışı
        [HttpPost("VerifyCodeAndRegister")]
        public async Task<IActionResult> VerifyCodeAndRegister(
            [FromBody] VerifyCodeAndRegisterRequestDto dto)
        {
            var cmd = new VerifyCodeAndRegisterCommand
            {
                Email = dto.Email,
                Code = dto.Code,
                FullName = dto.FullName,
                Password = dto.Password
            };
            var res = await _mediator.Send(cmd);
            return Ok(res);
        }

        // C) Şifre sıfırlama (yeni şifreyi set eder)
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequestDto dto)
        {
            var cmd = new ResetPasswordCommand
            {
                Email = dto.Email,
                Code = dto.Code,
                NewPassword = dto.NewPassword
            };
            var res = await _mediator.Send(cmd);
            return Ok(res);
        }

        [HttpPost("SendVerificationCode")]
        public async Task<IActionResult> SendVerificationCode(
    [FromBody] SendVerificationCodeRequestDto dto)
        {
            var res = await _mediator.Send(
                new SendVerificationCodeCommand { Email = dto.Email });
            return Ok(res);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _mediator.Send(new LoginCommand
            {
                Email = dto.Email,
                Password = dto.Password
            });
            return Ok(result);
        }
    }
}

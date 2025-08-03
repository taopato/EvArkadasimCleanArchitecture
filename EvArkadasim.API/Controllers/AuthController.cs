// EvArkadasim.API/Controllers/AuthController.cs
using Application.Features.Auths.Commands.SendVerificationCode;
using Application.Features.Auths.Commands.VerifyCode;
using Application.Features.Auths.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EvArkadasim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] SendVerificationCodeCommand cmd)
        {
            await _mediator.Send(cmd);
            return NoContent();
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyCodeCommand cmd)
        {
            AuthResultDto result = await _mediator.Send(cmd);
            return Ok(result);
        }

    }
}

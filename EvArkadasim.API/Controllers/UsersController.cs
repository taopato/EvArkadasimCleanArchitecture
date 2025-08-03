using Application.Features.Users.Commands.Create;
using Application.Features.Users.Dtos;
using Application.Features.Users.Queries.GetUserList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<UserDto> result = await _mediator.Send(new GetUserListQuery());
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Add(CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Created("", result);
        }

    }
}

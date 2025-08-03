using Application.Features.Houses.Commands.CreateHouse;
using Application.Features.Houses.Commands.AddHouseMember;
using Application.Features.Houses.Dtos;
using Application.Features.Houses.Queries.GetHouseDetail;
using Application.Features.Houses.Queries.GetHouseList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HousesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public HousesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _mediator.Send(new GetHouseListQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _mediator.Send(new GetHouseDetailQuery { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHouseCommand command)
        {
            var dto = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDetail), new { id = dto.Id }, dto);
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id, [FromBody] AddHouseMemberCommand command)
        {
            command.HouseId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int id, int userId)
        {
            await _mediator.Send(new AddHouseMemberCommand { HouseId = id, UserId = userId });
            return NoContent();
        }
    }
}

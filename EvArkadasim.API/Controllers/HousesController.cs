using Application.Features.Houses.Commands.AcceptInvitation;
using Application.Features.Houses.Commands.AddHouseMember;
using Application.Features.Houses.Commands.CreateHouse;
using Application.Features.Houses.Dtos;
using Application.Features.Houses.Queries.GetHouseDetail;
using Application.Features.Houses.Queries.GetHouseList;
using Application.Features.Houses.Queries.GetHouseMembersWithDebts;
using Application.Features.Houses.Queries.GetSpendingOverview;
using Application.Features.Houses.Queries.GetUserDebts;
using Application.Features.Houses.Queries.GetUserHouses;
using Application.Features.Invitations.Commands.SendInvitation;
using Application.Features.Invitations.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        [HttpPost("{houseId}/invitations")]
        public async Task<IActionResult> SendInvitation(int houseId, [FromBody] SendInvitationRequestDto request)
        {
            var command = new SendInvitationCommand
            {
                HouseId = houseId,
                Email = request.Email
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("AcceptInvitation")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success)
                return BadRequest("Davet kodu geçersiz veya zaman aşımına uğramış.");

            return Ok("Davet başarıyla kabul edildi.");
        }

        [HttpGet("{houseId}/members")]
        public async Task<IActionResult> GetHouseMembers(int houseId)
        {
            var result = await _mediator.Send(new GetHouseMembersWithDebtsQuery { HouseId = houseId });
            return Ok(result);
        }

        // ✅ Kullanıcının evlerini getir
        [HttpGet("GetUserHouses/{userId}")]
        public async Task<IActionResult> GetUserHouses(int userId)
        {
            var result = await _mediator.Send(new GetUserHousesQuery { UserId = userId });
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Data); // yalnız liste dönüyor
        }

        // ✅ Response sarma beklemeyecek şekilde sadeleştirildi
        [HttpGet("GetUserDebts/{userId:int}/{houseId:int}")]
        public async Task<IActionResult> GetUserDebts(int userId, int houseId)
        {
            var result = await _mediator.Send(new GetUserDebtsQuery(userId, houseId));
            return Ok(result);
        }

        // ✅ Tüm kullanıcılar için (ev geneli)
        [HttpGet("GetUserDebts/{houseId:int}")]
        public async Task<IActionResult> GetAllUsersDebts(int houseId)
        {
            var result = await _mediator.Send(new GetUserDebtsQuery { HouseId = houseId });
            return Ok(result);
        }

        [HttpGet("{houseId}/spending-overview")]
        public async Task<IActionResult> SpendingOverview(int houseId, DateTime? from, DateTime? to, int recentLimit = 10)
        {
            var res = await _mediator.Send(new GetSpendingOverviewQuery
            {
                HouseId = houseId,
                From = from,
                To = to,
                RecentLimit = recentLimit
            });
            return Ok(res);
        }
    }
}

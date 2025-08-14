using Application.Features.RecurringCharges.Commands.CreateRecurringCharge;
using Application.Features.RecurringCharges.Commands.UpdateRecurringCharge;
using Application.Features.RecurringCharges.Commands.CancelRecurringCharge;
using Application.Features.RecurringCharges.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EvArkadasim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecurringChargesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RecurringChargesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRecurringChargeRequest req, CancellationToken ct)
        {
            var res = await _mediator.Send(new CreateRecurringChargeCommand(req), ct);
            return Ok(res);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRecurringChargeRequest req, CancellationToken ct)
        {
            var res = await _mediator.Send(new UpdateRecurringChargeCommand(id, req), ct);
            return Ok(res);
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int id, [FromBody] CancelRecurringChargeRequest req, CancellationToken ct)
        {
            var res = await _mediator.Send(new CancelRecurringChargeCommand(id, req), ct);
            return Ok(res);
        }
    }
}

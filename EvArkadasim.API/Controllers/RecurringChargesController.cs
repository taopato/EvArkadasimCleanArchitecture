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
    }
}

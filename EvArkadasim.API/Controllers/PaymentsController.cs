// EvArkadasim.API/Controllers/PaymentsController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Payments.Commands.CreatePayment;
using Application.Features.Payments.Queries.GetPaymentList;

namespace EvArkadasim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost("AddPayment")]
        public async Task<IActionResult> AddPayment(
            [FromBody] CreatePaymentCommand cmd)
        {
            var res = await _mediator.Send(cmd);
            return Created(string.Empty, res);
        }

        [HttpGet("GetPayments/{houseId}")]
        public async Task<IActionResult> GetPayments(int houseId)
        {
            var list = await _mediator.Send(
                new GetPaymentListQuery { HouseId = houseId });
            return Ok(list);
        }
    }
}

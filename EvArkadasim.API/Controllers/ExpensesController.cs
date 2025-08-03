using Application.Features.Expenses.Commands.CreateExpense;
using Application.Features.Expenses.Dtos;
using Application.Features.Expenses.Queries.GetExpenseList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpensesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            List<ExpenseDto> result = await _mediator.Send(new GetExpenseListQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseCommand command)
        {
            CreatedExpenseDto result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetList), new { id = result.Id }, result);
        }
    }
}

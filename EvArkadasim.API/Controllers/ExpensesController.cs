using Application.Features.Expenses.Commands.CreateExpense;
using Application.Features.Expenses.Dtos;
using Application.Features.Expenses.Queries.GetExpenseList;
using Application.Features.Expenses.Queries.GetExpensesByHouse;
using Application.Features.Expenses.Queries.GetExpense;
using Application.Features.Expenses.Commands.DeleteExpense;
using Application.Features.Expenses.Commands.UpdateExpense;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results;

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

        // 1) Mevcut listeleme (Dto listesi döner)
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            Response<List<ExpenseDto>> result =
                await _mediator.Send(new GetExpenseListQuery());

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // 2) Mevcut oluşturma (CreatedExpenseResponseDto döner)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseCommand command)
        {
            CreatedExpenseResponseDto result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetList), new { id = result.Id }, result);
        }

        // 3) Mevcut AddExpense alias’ı
        [HttpPost("AddExpense")]
        public async Task<IActionResult> AddExpense([FromBody] CreateExpenseCommand command)
        {
            CreatedExpenseResponseDto result = await _mediator.Send(command);
            return CreatedAtAction(nameof(AddExpense), new { id = result.Id }, result);
        }

        // ──────────────────────────────────────────────────────
        // 4) Yeni: Harcama Listesi by House (wrapper ile döner)
        [HttpGet("GetExpenses/{houseId}")]
        public async Task<IActionResult> GetExpenses(int houseId)
        {
            var data = await _mediator.Send(
                new GetExpensesByHouseQuery { HouseId = houseId });
            return Ok(new { isSuccess = true, data });
        }

        // 5) Yeni: Harcama Detayı by Id
        [HttpGet("GetExpense/{expenseId}")]
        public async Task<IActionResult> GetExpense(int expenseId)
        {
            var data = await _mediator.Send(
                new GetExpenseQuery { ExpenseId = expenseId });
            return Ok(new { isSuccess = true, data });
        }

        // 6) Yeni: Harcama Silme
        [HttpDelete("DeleteExpense/{expenseId}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
            await _mediator.Send(
                new DeleteExpenseCommand { ExpenseId = expenseId });
            return Ok(new { isSuccess = true, message = "Harcama başarıyla silindi." });
        }

        // 7) Yeni: Harcama Güncelleme
        [HttpPut("UpdateExpense/{expenseId}")]
        public async Task<IActionResult> UpdateExpense(
            int expenseId,
            [FromBody] UpdateExpenseDto dto)
        {
            await _mediator.Send(new UpdateExpenseCommand
            {
                ExpenseId = expenseId,
                Dto = dto
            });
            return Ok(new { isSuccess = true, message = "Harcama başarıyla güncellendi." });
        }
    }
}

using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Bills.Commands.CreateBill;
using Application.Features.Bills.Commands.FinalizeBill;
using Application.Features.Bills.Commands.UploadBillDocument;
using Application.Features.Bills.Dtos;
using Application.Features.Bills.Queries.GetRecentBills;
using Domain.Enums;


namespace EvArkadasim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public BillsController(IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        [HttpPost]
        [Authorize] // isterseniz açın
        public async Task<ActionResult<BillDetailDto>> Create([FromBody] CreateBillDto dto)
        {
            var res = await _mediator.Send(new CreateBillCommand { Model = dto });
            return Ok(res);
        }

        [HttpPost("{billId}/finalize")]
        [Authorize]
        public async Task<IActionResult> FinalizeBill([FromRoute] int billId, [FromQuery] int requestUserId)
        {
            var ok = await _mediator.Send(new FinalizeBillCommand
            {
                BillId = billId,
                RequestUserId = requestUserId
            });
            return Ok(new { ok });
        }

        // Form-Data: file
        [HttpPost("{billId}/documents")]
        [Authorize]
        public async Task<IActionResult> UploadDocument([FromRoute] int billId, IFormFile file, [FromQuery] int requestUserId)
        {
            if (file == null || file.Length == 0) return BadRequest("Dosya yok.");

            var uploads = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "bills");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var safeName = $"{billId}_{System.Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploads, safeName);

            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            var relative = $"/uploads/bills/{safeName}"; // Static files middleware açık olmalı

            var docId = await _mediator.Send(new UploadBillDocumentCommand
            {
                BillId = billId,
                RequestUserId = requestUserId,
                FileName = file.FileName,
                FilePathOrUrl = relative
            });

            return Ok(new { documentId = docId, url = relative });
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int houseId, [FromQuery] UtilityType? utilityType, [FromQuery] int limit = 10)
        {
            var list = await _mediator.Send(new GetRecentBillsQuery
            {
                HouseId = houseId,
                UtilityType = utilityType,
                Limit = limit
            });
            return Ok(list);
        }
    }
}

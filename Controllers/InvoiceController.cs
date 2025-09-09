using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using InvoiceApi.Services;
using InvoiceApi.DTOs;

namespace InvoiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _svc;
        public InvoiceController(InvoiceService svc) => _svc = svc;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateDto dto)
        {
            try
            {
                var id = await _svc.CreateInvoiceAsync(dto);
                return CreatedAtAction(nameof(GetByNumber), new { number = dto.InvoiceNumber }, new { InvoiceId = id });
            }
            catch (ArgumentException aex)
            {
                return BadRequest(new { error = aex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{number}")]
        public async Task<IActionResult> GetByNumber(string number)
        {
            var invoice = await _svc.GetInvoiceByNumberAsync(number);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string type, [FromQuery] int? clientId, [FromQuery] string? number, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var res = await _svc.SearchInvoicesAsync(type, clientId, number, from, to);
            return Ok(res);
        }
    }
}
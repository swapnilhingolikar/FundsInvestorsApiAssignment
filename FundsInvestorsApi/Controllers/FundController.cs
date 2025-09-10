using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundsInvestorsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundController : ControllerBase
    {
        private readonly IFundService _service;
        public FundController(IFundService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var fund = await _service.GetByIdAsync(id);
            if (fund == null) return NotFound();
            return Ok(fund);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FundCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.FundId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FundUpdateDto dto)
        {
            if (id != dto.FundId) return BadRequest();
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}

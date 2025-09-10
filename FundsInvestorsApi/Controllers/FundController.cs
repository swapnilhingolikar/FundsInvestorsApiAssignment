using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundsInvestorsApi.Controllers
{
    /// <summary>
    /// API controller responsible for handling Fund-related HTTP requests.
    /// Provides endpoints for CRUD operations on funds.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FundController : ControllerBase
    {
        private readonly IFundService _service;

        /// <summary>
        /// Constructor with dependency injection for FundService.
        /// </summary>
        /// <param name="service">Service for fund business logic.</param>
        public FundController(IFundService service) => _service = service;

        /// <summary>
        /// Retrieves all funds.
        /// </summary>
        /// <returns>List of funds.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        /// <summary>
        /// Retrieves a fund by its ID.
        /// </summary>
        /// <param name="id">Unique identifier of the fund.</param>
        /// <returns>Fund details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var fund = await _service.GetByIdAsync(id);
            if (fund == null) return NotFound(); // Return 404 if not found
            return Ok(fund);
        }

        /// <summary>
        /// Creates a new fund.
        /// </summary>
        /// <param name="dto">Fund details for creation.</param>
        /// <returns>Created fund with 201 status code.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(FundCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            // Return 201 Created with location header
            return CreatedAtAction(nameof(GetById), new { id = created.FundId }, created);
        }

        /// <summary>
        /// Updates an existing fund.
        /// </summary>
        /// <param name="id">Fund ID to update.</param>
        /// <param name="dto">Updated fund details.</param>
        /// <returns>NoContent if update successful, otherwise BadRequest.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, FundUpdateDto dto)
        {
            if (id != dto.FundId) return BadRequest(); // Ensure route ID matches DTO ID
            await _service.UpdateAsync(dto);
            return NoContent(); // Return 204 No Content
        }

        /// <summary>
        /// Deletes a fund by ID.
        /// </summary>
        /// <param name="id">Fund ID.</param>
        /// <returns>NoContent if deletion successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent(); // Return 204 No Content
        }
    }
}

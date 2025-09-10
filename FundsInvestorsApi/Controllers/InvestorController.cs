using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FundsInvestorsApi.Controllers
{
    /// <summary>
    /// API controller responsible for handling Investor-related HTTP requests.
    /// Provides endpoints for CRUD operations on investors.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvestorController : ControllerBase
    {
        private readonly IInvestorService _service;

        /// <summary>
        /// Constructor with dependency injection for InvestorService.
        /// </summary>
        /// <param name="service">Service for investor business logic.</param>
        public InvestorController(IInvestorService service) => _service = service;

        /// <summary>
        /// Retrieves all investors.
        /// </summary>
        /// <returns>List of investors.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        /// <summary>
        /// Retrieves an investor by their ID.
        /// </summary>
        /// <param name="id">Unique identifier of the investor.</param>
        /// <returns>Investor details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var investor = await _service.GetByIdAsync(id);
            if (investor == null) return NotFound(); // Return 404 if not found
            return Ok(investor);
        }

        /// <summary>
        /// Creates a new investor.
        /// </summary>
        /// <param name="dto">Investor details for creation.</param>
        /// <returns>Created investor with 201 status code.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(InvestorCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            // Return 201 Created with location header
            return CreatedAtAction(nameof(GetById), new { id = created.InvestorId }, created);
        }

        /// <summary>
        /// Updates an existing investor.
        /// </summary>
        /// <param name="id">Investor ID to update.</param>
        /// <param name="dto">Updated investor details.</param>
        /// <returns>NoContent if update successful, otherwise BadRequest.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, InvestorUpdateDto dto)
        {
            if (id != dto.InvestorId) return BadRequest(); // Ensure route ID matches DTO ID
            await _service.UpdateAsync(dto);
            return NoContent(); // Return 204 No Content
        }

        /// <summary>
        /// Deletes an investor by ID.
        /// </summary>
        /// <param name="id">Investor ID.</param>
        /// <returns>NoContent if deletion successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent(); // Return 204 No Content
        }
    }
}

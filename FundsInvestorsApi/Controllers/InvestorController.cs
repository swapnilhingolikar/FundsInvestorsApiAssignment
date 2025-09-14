using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var investors = await _service.GetAllAsync();
                return Ok(investors);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching all investors");
                return StatusCode(500, new { message = "An error occurred while fetching investors." });
            }
        }

        /// <summary>
        /// Retrieves an investor by their ID.
        /// </summary>
        /// <param name="id">Unique identifier of the investor.</param>
        /// <returns>Investor details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var investor = await _service.GetByIdAsync(id);
                if (investor == null)
                {
                    Log.Warning("Investor with ID {InvestorId} not found", id);
                    return NotFound(new { message = "Investor not found." });
                }

                return Ok(investor);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching investor with ID {InvestorId}", id);
                return StatusCode(500, new { message = "An error occurred while fetching the investor." });
            }
        }

        /// <summary>
        /// Creates a new investor.
        /// </summary>
        /// <param name="dto">Investor details for creation.</param>
        /// <returns>Created investor with 201 status code.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(InvestorCreateDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                Log.Information("Investor created with ID {InvestorId}", created.InvestorId);
                return CreatedAtAction(nameof(GetById), new { id = created.InvestorId }, created);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating investor");
                return StatusCode(500, new { message = "An error occurred while creating the investor." });
            }
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
            try
            {
                if (id != dto.InvestorId)
                {
                    Log.Warning("Investor ID mismatch: route {RouteId}, body {BodyId}", id, dto.InvestorId);
                    return BadRequest(new { message = "ID mismatch between route and request body." });
                }

                await _service.UpdateAsync(dto);
                Log.Information("Investor with ID {InvestorId} updated", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating investor with ID {InvestorId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the investor." });
            }
        }

        /// <summary>
        /// Deletes an investor by ID.
        /// </summary>
        /// <param name="id">Investor ID.</param>
        /// <returns>Success message if deletion successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var investor = await _service.GetByIdAsync(id);
                if (investor == null)
                {
                    Log.Warning("Attempted to delete non-existing investor with ID {InvestorId}", id);
                    return NotFound(new { message = "Investor not found." });
                }

                await _service.DeleteAsync(id);
                Log.Information("Investor with ID {InvestorId} deleted", id);
                return Ok(new { message = "Investor deleted successfully.", investorId = id });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting investor with ID {InvestorId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the investor." });
            }
        }
    }
}

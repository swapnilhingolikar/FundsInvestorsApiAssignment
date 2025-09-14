using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FundsInvestorsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundController : ControllerBase
    {
        private readonly IFundService _service;
        private readonly ILogger<FundController> _logger;

        /// <summary>
        /// Constructor with dependency injection of service and logger
        /// </summary>
        public FundController(IFundService service, ILogger<FundController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all funds.
        /// </summary>
        /// <returns>200 OK with list of funds, or 500 if error occurs</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var funds = await _service.GetAllAsync();
                _logger.LogInformation("Retrieved {Count} funds", funds.Count());
                return Ok(funds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all funds");
                return StatusCode(500, "An error occurred while fetching funds.");
            }
        }

        /// <summary>
        /// Retrieves a specific fund by ID.
        /// </summary>
        /// <param name="id">Fund ID</param>
        /// <returns>200 OK with fund if found, 404 if not found, 500 if error occurs</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var fund = await _service.GetByIdAsync(id);
                if (fund == null)
                {
                    _logger.LogWarning("Fund with ID {FundId} not found", id);
                    return NotFound(new { message = "Fund not found." });
                }

                _logger.LogInformation("Retrieved fund with ID {FundId}", id);
                return Ok(fund);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fund with ID {FundId}", id);
                return StatusCode(500, "An error occurred while fetching the fund.");
            }
        }

        /// <summary>
        /// Creates a new fund.
        /// </summary>
        /// <param name="dto">Fund creation details</param>
        /// <returns>201 Created with fund, 400 if validation fails, 500 if error occurs</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FundCreateDto dto)
        {
            // Model validation check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid fund creation attempt: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _service.CreateAsync(dto);
                _logger.LogInformation("Created new fund with ID {FundId}", created.FundId);
                return CreatedAtAction(nameof(GetById), new { id = created.FundId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fund {@FundCreateDto}", dto);
                return StatusCode(500, "An error occurred while creating the fund.");
            }
        }

        /// <summary>
        /// Updates an existing fund.
        /// </summary>
        /// <param name="id">Fund ID</param>
        /// <param name="dto">Updated fund details</param>
        /// <returns>204 NoContent if successful, 400 if validation fails or ID mismatch, 500 if error occurs</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FundUpdateDto dto)
        {
            // Model validation check
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid fund update attempt: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            // Ensure route ID matches DTO ID
            if (id != dto.FundId)
            {
                _logger.LogWarning("Mismatch between route ID {RouteId} and DTO ID {DtoId}", id, dto.FundId);
                return BadRequest("Fund ID mismatch");
            }

            try
            {
                await _service.UpdateAsync(dto);
                _logger.LogInformation("Updated fund with ID {FundId}", id);
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fund with ID {FundId}", id);
                return StatusCode(500, "An error occurred while updating the fund.");
            }
        }

        /// <summary>
        /// Deletes a fund by ID.
        /// </summary>
        /// <param name="id">Fund ID</param>
        /// <returns>200 OK with success message if deleted, 404 if not found, 500 if error occurs</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Check if fund exists before deleting
                var fund = await _service.GetByIdAsync(id);
                if (fund == null)
                {
                    _logger.LogWarning("Attempted to delete non-existing fund with ID {FundId}", id);
                    return NotFound(new { message = "Fund not found." });
                }

                await _service.DeleteAsync(id);
                _logger.LogInformation("Deleted fund with ID {FundId}", id);

                // Return success message
                return Ok(new
                {
                    message = "Fund deleted successfully.",
                    fundId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fund with ID {FundId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the fund." });
            }
        }
    }
}
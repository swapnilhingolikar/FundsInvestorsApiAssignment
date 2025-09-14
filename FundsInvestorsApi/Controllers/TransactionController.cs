using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FundsInvestorsApi.Controllers
{
    /// <summary>
    /// API controller responsible for handling Transaction-related HTTP requests.
    /// Provides endpoints for retrieving and creating transactions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        /// <summary>
        /// Constructor with dependency injection for TransactionService.
        /// </summary>
        /// <param name="service">Service for transaction business logic.</param>
        public TransactionController(ITransactionService service) => _service = service;

        /// <summary>
        /// Retrieves all transactions.
        /// </summary>
        /// <returns>List of transactions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var transactions = await _service.GetAllAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching all transactions");
                return StatusCode(500, new { message = "An error occurred while fetching transactions." });
            }
        }

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="id">Unique identifier of the transaction.</param>
        /// <returns>Transaction details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                Log.Warning("Invalid transaction ID provided: {TransactionId}", id);
                return BadRequest(new { message = "Transaction ID is required." });
            }

            try
            {
                var transaction = await _service.GetByIdAsync(id);
                if (transaction == null)
                {
                    Log.Warning("Transaction with ID {TransactionId} not found", id);
                    return NotFound(new { message = "Transaction not found." });
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching transaction with ID {TransactionId}", id);
                return StatusCode(500, new { message = "An error occurred while fetching the transaction." });
            }
        }


        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="dto">Transaction details for creation.</param>
        /// <returns>Created transaction with 201 status code.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateDto dto)
        {
            // Check if the incoming model is valid
            if (!ModelState.IsValid)
            {
                // Return 400 Bad Request with validation errors
                Log.Warning("Invalid transaction data: {@TransactionDto}", dto);
                return BadRequest(ModelState);
            }

            try
            {
                var created = await _service.CreateAsync(dto);
                Log.Information("Transaction created with ID {TransactionId}", created.TransactionId);
                return CreatedAtAction(nameof(GetById), new { id = created.TransactionId }, created);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating transaction");
                return StatusCode(500, new { message = "An error occurred while creating the transaction." });
            }
        }
    }
}

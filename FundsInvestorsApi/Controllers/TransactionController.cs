using FundsInvestorsApi.DTOs;
using FundsInvestorsApi.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        /// <summary>
        /// Retrieves a transaction by its ID.
        /// </summary>
        /// <param name="id">Unique identifier of the transaction.</param>
        /// <returns>Transaction details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var transaction = await _service.GetByIdAsync(id);
            if (transaction == null) return NotFound(); // Return 404 if not found
            return Ok(transaction);
        }

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="dto">Transaction details for creation.</param>
        /// <returns>Created transaction with 201 status code.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            // Return 201 Created with location header
            return CreatedAtAction(nameof(GetById), new { id = created.TransactionId }, created);
        }
    }
}

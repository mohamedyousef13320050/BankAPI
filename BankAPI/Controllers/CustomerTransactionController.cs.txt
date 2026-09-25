using System.Security.Claims;
using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Customer self-service transaction operations (View history and Inter-account atomic transfer)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public class CustomerTransactionController : ControllerBase
    {
        private readonly ITransactionBL transactionBL;

        public CustomerTransactionController(ITransactionBL transactionBL)
        {
            this.transactionBL = transactionBL;
        }

        /// <summary>
        /// Get transaction history for an owned bank account.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        [HttpGet("history/{accountId}")]
        public IActionResult History(int accountId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User ID not found in token claims." });

            var history = transactionBL.GetCustomerTransactions(userId, accountId);
            if (history == null) return NotFound(new { Message = "Account not found or access denied." });

            return Ok(history);
        }

        /// <summary>
        /// Execute an atomic inter-account transfer using a database transaction (BeginTransaction + Rollback on failure).
        /// </summary>
        /// <param name="model">Transfer parameters (FromAccountId, ToAccountId, Amount, Description)</param>
        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User ID not found in token claims." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = transactionBL.ProcessTransfer(userId, model);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
    }
}

using System.Security.Claims;
using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
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

        [HttpGet("history/{accountId}")]
        public IActionResult History(int accountId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User ID not found in token claims." });

            var history = transactionBL.GetCustomerTransactions(userId, accountId);
            if (history == null) return NotFound(new { Message = "Account not found or access denied." });

            return Ok(history);
        }

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

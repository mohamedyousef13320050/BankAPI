using System.Security.Claims;
using BankSystem.BL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Customer self-service operations for viewing owned bank accounts and balances.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public class CustomerAccountController : ControllerBase
    {
        private readonly IAccountBL accountBL;

        public CustomerAccountController(IAccountBL accountBL)
        {
            this.accountBL = accountBL;
        }

        /// <summary>
        /// Get all bank accounts owned by the logged-in customer.
        /// </summary>
        [HttpGet("my-accounts")]
        public IActionResult GetMyAccounts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User ID not found in token claims." });

            var accounts = accountBL.GetMyAccounts(userId);
            return Ok(accounts);
        }

        /// <summary>
        /// Get account details for a specific owned account ID.
        /// </summary>
        /// <param name="id">Account ID</param>
        [HttpGet("{id}")]
        public IActionResult GetDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User ID not found in token claims." });

            var account = accountBL.GetMyAccountDetails(userId, id);
            if (account == null) return NotFound(new { Message = "Account not found or does not belong to you." });

            return Ok(account);
        }
    }
}

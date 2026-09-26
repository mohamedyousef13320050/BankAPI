using System.Security.Claims;
using BankSystem.BL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
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

        [HttpGet("my-accounts")]
        public IActionResult GetMyAccounts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "User ID not found in token claims." });

            var accounts = accountBL.GetMyAccounts(userId);
            return Ok(accounts);
        }

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

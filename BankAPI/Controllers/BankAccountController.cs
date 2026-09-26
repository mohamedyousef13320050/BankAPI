using BankSystem.BL;
using BankSystem.Models;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Employee and Admin operations for managing customer bank accounts (Create, View, Block, Close)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Employee")]
    public class BankAccountController : ControllerBase
    {
        private readonly IAccountBL accountBL;

        public BankAccountController(IAccountBL accountBL)
        {
            this.accountBL = accountBL;
        }

        /// <summary>
        /// Get list of all bank accounts.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var accounts = accountBL.GetAllAccounts();
            return Ok(accounts);
        }

        /// <summary>
        /// Get bank account details by ID.
        /// </summary>
        /// <param name="id">Account ID</param>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var account = accountBL.GetAccountDetails(id);
            if (account == null) return NotFound(new { Message = "Account not found." });
            return Ok(account);
        }

        /// <summary>
        /// Create a new bank account for a customer.
        /// </summary>
        /// <param name="model">Account creation parameters</param>
        [HttpPost]
        public IActionResult Create([FromBody] AccountCreateVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors) = accountBL.CreateAccount(model);
            if (success)
            {
                return Ok(new { Message = "Bank account created successfully." });
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Change account status (Active, Blocked, Closed).
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="status">Target status enum value (Active=1, Blocked=2, Closed=3)</param>
        [HttpPatch("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromQuery] AccountStatus status)
        {
            var account = accountBL.GetAccountDetails(id);
            if (account == null) return NotFound(new { Message = "Account not found." });

            accountBL.ChangeAccountStatus(id, status);
            return Ok(new { Message = $"Account status changed to {status} successfully." });
        }
    }
}

using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Employee counter operations for Deposits and Withdrawals.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Employee")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionBL transactionBL;

        public TransactionController(ITransactionBL transactionBL)
        {
            this.transactionBL = transactionBL;
        }

        /// <summary>
        /// Process cash deposit into a active bank account.
        /// </summary>
        /// <param name="model">Deposit parameters (AccountId, Amount, Description)</param>
        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] DepositVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = transactionBL.ProcessDeposit(model);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }

        /// <summary>
        /// Process cash withdrawal from an active bank account (verifies non-negative balance constraint).
        /// </summary>
        /// <param name="model">Withdrawal parameters (AccountId, Amount, Description)</param>
        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] WithdrawVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = transactionBL.ProcessWithdraw(model);
            if (success)
            {
                return Ok(new { Message = message });
            }

            return BadRequest(new { Message = message });
        }
    }
}

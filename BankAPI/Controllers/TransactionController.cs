using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
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

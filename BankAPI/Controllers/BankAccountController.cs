using BankSystem.BL;
using BankSystem.Models;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var accounts = accountBL.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var account = accountBL.GetAccountDetails(id);
            if (account == null) return NotFound(new { Message = "Account not found." });
            return Ok(account);
        }

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

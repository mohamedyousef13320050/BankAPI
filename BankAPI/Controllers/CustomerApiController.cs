using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Employee")]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerBL customerBL;

        public CustomerApiController(ICustomerBL customerBL)
        {
            this.customerBL = customerBL;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var customers = customerBL.GetAllCustomers();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var customer = customerBL.GetCustomerById(id);
            if (customer == null)
                return NotFound(new { Message = "Customer not found." });

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors) = await customerBL.CreateCustomer(model);
            if (success)
                return Ok(new { Message = "Customer created successfully." });

            foreach (var error in errors)
                ModelState.AddModelError("", error);

            return BadRequest(ModelState);
        }

        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            var customer = customerBL.GetCustomerById(id);
            if (customer == null)
                return NotFound(new { Message = "Customer not found." });

            customerBL.ActivateCustomer(id);
            return Ok(new { Message = "Customer activated successfully." });
        }

        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            var customer = customerBL.GetCustomerById(id);
            if (customer == null)
                return NotFound(new { Message = "Customer not found." });

            customerBL.DeactivateCustomer(id);
            return Ok(new { Message = "Customer deactivated successfully." });
        }
    }
}

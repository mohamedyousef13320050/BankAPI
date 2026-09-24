using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Admin operations for managing bank employees (Create, Edit, List, Activate, Deactivate)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeBL employeeBL;

        public EmployeeController(IEmployeeBL employeeBL)
        {
            this.employeeBL = employeeBL;
        }

        /// <summary>
        /// Get list of all bank employees.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var employees = employeeBL.GetAllEmployees();
            return Ok(employees);
        }

        /// <summary>
        /// Get employee details by ID.
        /// </summary>
        /// <param name="id">Employee ID</param>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var employee = employeeBL.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound(new { Message = "Employee not found." });
            }
            return Ok(employee);
        }

        /// <summary>
        /// Create a new bank employee and seed their Identity login user.
        /// </summary>
        /// <param name="model">Employee creation parameters</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors) = await employeeBL.CreateEmployee(model);
            if (success)
            {
                return Ok(new { Message = "Employee created successfully." });
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update employee details.
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="model">Employee edit parameters</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EmployeeEditVM model)
        {
            if (id != model.Id)
            {
                return BadRequest(new { Message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errors) = await employeeBL.UpdateEmployee(model);
            if (success)
            {
                return Ok(new { Message = "Employee updated successfully." });
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError("", error);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Activate an employee account.
        /// </summary>
        /// <param name="id">Employee ID</param>
        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            var emp = employeeBL.GetEmployeeById(id);
            if (emp == null) return NotFound(new { Message = "Employee not found." });

            employeeBL.ActivateEmployee(id);
            return Ok(new { Message = "Employee activated successfully." });
        }

        /// <summary>
        /// Deactivate an employee account.
        /// </summary>
        /// <param name="id">Employee ID</param>
        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            var emp = employeeBL.GetEmployeeById(id);
            if (emp == null) return NotFound(new { Message = "Employee not found." });

            employeeBL.DeactivateEmployee(id);
            return Ok(new { Message = "Employee deactivated successfully." });
        }
    }
}

using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Admin operations for managing bank branches.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchBL branchBL;

        public BranchController(IBranchBL branchBL)
        {
            this.branchBL = branchBL;
        }

        /// <summary>
        /// Get list of all bank branches.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var branches = branchBL.GetAllBranches();
            return Ok(branches);
        }

        /// <summary>
        /// Get branch details by ID.
        /// </summary>
        /// <param name="id">Branch ID</param>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var branch = branchBL.GetBranchById(id);
            if (branch == null) return NotFound(new { Message = "Branch not found." });
            return Ok(branch);
        }

        /// <summary>
        /// Create a new bank branch.
        /// </summary>
        /// <param name="model">Branch creation parameters</param>
        [HttpPost]
        public IActionResult Create([FromBody] BranchCreateVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            branchBL.CreateBranch(model);
            return Ok(new { Message = "Branch created successfully." });
        }

        /// <summary>
        /// Update branch details.
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <param name="model">Branch edit parameters</param>
        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] BranchEditVM model)
        {
            if (id != model.Id)
                return BadRequest(new { Message = "ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = branchBL.UpdateBranch(model);
            if (success)
                return Ok(new { Message = "Branch updated successfully." });

            return BadRequest(new { Message = "Failed to update branch." });
        }

        /// <summary>
        /// Activate a bank branch.
        /// </summary>
        /// <param name="id">Branch ID</param>
        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            var branch = branchBL.GetBranchById(id);
            if (branch == null) return NotFound(new { Message = "Branch not found." });

            branchBL.ActivateBranch(id);
            return Ok(new { Message = "Branch activated successfully." });
        }

        /// <summary>
        /// Deactivate a bank branch.
        /// </summary>
        /// <param name="id">Branch ID</param>
        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            var branch = branchBL.GetBranchById(id);
            if (branch == null) return NotFound(new { Message = "Branch not found." });

            branchBL.DeactivateBranch(id);
            return Ok(new { Message = "Branch deactivated successfully." });
        }
    }
}

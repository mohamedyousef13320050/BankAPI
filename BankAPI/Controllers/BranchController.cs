using BankSystem.BL;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var branches = branchBL.GetAllBranches();
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var branch = branchBL.GetBranchById(id);
            if (branch == null) return NotFound(new { Message = "Branch not found." });
            return Ok(branch);
        }

        [HttpPost]
        public IActionResult Create([FromBody] BranchCreateVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            branchBL.CreateBranch(model);
            return Ok(new { Message = "Branch created successfully." });
        }

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

        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            var branch = branchBL.GetBranchById(id);
            if (branch == null) return NotFound(new { Message = "Branch not found." });

            branchBL.ActivateBranch(id);
            return Ok(new { Message = "Branch activated successfully." });
        }

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

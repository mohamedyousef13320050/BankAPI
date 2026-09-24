using BankSystem.Models;
using BankSystem.Services;
using BankSystem.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Controllers
{
    /// <summary>
    /// Authentication and Identity Operations (Login, Register, JWT Token Generation)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IJwtTokenService tokenService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        /// <summary>
        /// Authenticate user and return a signed JWT Bearer Token with roles and expiration.
        /// </summary>
        /// <param name="model">Username/Email and Password credentials</param>
        /// <returns>JWT Bearer token and user claims</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(model.UserName);
            }

            if (user != null)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    var (token, expiration) = tokenService.GenerateToken(user, roles);

                    return Ok(new
                    {
                        Token = token,
                        Expiration = expiration,
                        UserId = user.Id,
                        Username = user.UserName,
                        FullName = user.FullName,
                        Roles = roles
                    });
                }
            }

            return Unauthorized(new { Message = "Invalid username or password." });
        }

        /// <summary>
        /// Register a new Customer account.
        /// </summary>
        /// <param name="model">Registration details (Username, Email, Password)</param>
        /// <returns>Registration status and new JWT token</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.UserName
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Customer");
                var roles = new List<string> { "Customer" };
                var (token, expiration) = tokenService.GenerateToken(user, roles);

                return Ok(new
                {
                    Message = "User registered successfully.",
                    Token = token,
                    Expiration = expiration,
                    UserId = user.Id,
                    Username = user.UserName
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}

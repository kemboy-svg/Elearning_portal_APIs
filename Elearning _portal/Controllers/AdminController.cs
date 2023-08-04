
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Elearning__portal.Models;

namespace Elearning__portal.Controllers
{
    [ApiController]
    [Route("api/Admin/Login")]
    public class AdminController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                // Authentication successful 
                return StatusCode(200, "Logged in successfully");
            }

            return Unauthorized("Invalid login attempt");
        }

        [HttpGet]
        [Route("GetAdminByEmail")]

        public async Task<IActionResult> GetAdmin(string email)
        {
            var admin = await _userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                return NotFound("User not found");
            }
            return Ok(admin);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Elearning__portal.Models;
using Microsoft.AspNetCore.Authorization;
using Elearning__portal.Data;

namespace Elearning__portal.Controllers
{
    [ApiController]
    [Route("api/Admin/Login")]
    public class AdminController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DtabaseSet _dtabaseSet;

        public AdminController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, DtabaseSet dtabaseSet)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dtabaseSet = dtabaseSet;
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

        [HttpPost]
        [Route("post/Announcements")]

        public async Task<IActionResult> PostAnnouncements( Announcements model)
        {
            try
            {
                

                var message = new Announcements
                {
                    Message = model.Message,
                    

                };
                _dtabaseSet.Announcements.Add(message);
                await _dtabaseSet.SaveChangesAsync();
                return Ok("Announcent sent successfully");
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while sending the message" + ex.Message);
            }
        }
    }


}
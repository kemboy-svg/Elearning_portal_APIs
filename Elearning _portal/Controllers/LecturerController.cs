
using Elearning__portal.Data;
using Elearning__portal.Models;
using Elearning__portal.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace Elearning__portal.Controllers
{
    public class LecturerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DtabaseSet _dtabaseSet;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LecturerController(UserManager<ApplicationUser> userManager, DtabaseSet dtabaseSet,
            RoleManager<IdentityRole>roleManager)
        {
            _userManager = userManager;
            _dtabaseSet = dtabaseSet;
            _roleManager = roleManager;
        }
        //  public LecturerController()
        // {
        // }

        private async Task CreateRolesIfNotExists(params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // create the roles and seed them to the database
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }


        
        [HttpPost]
        [Route("api/Register")]
        public async Task<IActionResult> Add(RegisterDTO model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    fullName = model.fullName,
                    UserName=model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var newL = new Lecturer
                    {
                        Id = model.Id,
                        fullName = model.fullName,
                        Email = model.Email,
                        assigned_unit = model.assigned_unit
                    };

                    await _dtabaseSet.Lecturers.AddAsync(newL);
                    await _dtabaseSet.SaveChangesAsync();
                    await CreateRolesIfNotExists("Lecturer");
                    await _userManager.AddToRoleAsync(user, "Lecturer");

                    return StatusCode(200, "Registered Successfully");
                }
                else
                {
                    return BadRequest("Failed to register");
                }
            }
            catch (Exception ex)
            {
          
                return StatusCode(500, "An error occurred while registering the user: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("api/Lecturer/{id}")]
        public async Task<IActionResult> UpdateLecturer(int id, RegisterDTO model)
        {
            try
            {
                var lecturer = await _dtabaseSet.Lecturers.FindAsync(id);

                if (lecturer == null)
                {
                    return NotFound("Lecturer not found");
                }

                lecturer.fullName = model.fullName;
                lecturer.Email  = model.Email;
                lecturer.assigned_unit = model.assigned_unit;

                _dtabaseSet.Lecturers.Update(lecturer);
                await _dtabaseSet.SaveChangesAsync();

                return StatusCode(200, "Lecturer updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the lecturer: " + ex.Message);
            }
        }
        [HttpDelete]
        [Route("api/Lecturer/{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            try
            {
                var lecturer = await _dtabaseSet.Lecturers.FindAsync(id);

                if (lecturer == null)
                {
                    return NotFound("Lecturer not found");
                }

                _dtabaseSet.Lecturers.Remove(lecturer);
                await _dtabaseSet.SaveChangesAsync();

                return StatusCode(200, "Lecturer deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the lecturer: " + ex.Message);
            }
        }


        [HttpGet]
        [Route("api/lecturers")]

        public async Task<IActionResult> Lecturers()
        {
           var users = await _dtabaseSet.Lecturers.ToListAsync();
            return Ok(users);
        }


    }
}

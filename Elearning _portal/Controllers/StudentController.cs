using Elearning__portal.Data;
using Elearning__portal.Models;
using Elearning__portal.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearning__portal.Controllers
{
    public class StudentController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DtabaseSet _dtabaseSet;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StudentController(UserManager<Models.ApplicationUser> userManager, DtabaseSet dtabaseSet,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _dtabaseSet = dtabaseSet;
            _roleManager = roleManager;
        }

        private async Task CreateRolesIfNotExists(params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                 
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }



        [HttpPost]
        [Route("api/StdentRegister")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    fullName = model.fullName,
                    UserName = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var newS = new Student
                    {
                        Id = model.Id,
                        Reg_no = model.Reg_no,
                        fullName = model.fullName,
                        Email = model.Email,
                        Course =model.Course,
                        Age=model.Age
                      
                    };
                    await _dtabaseSet.Students.AddAsync(newS);
                    await _dtabaseSet.SaveChangesAsync();
                    await CreateRolesIfNotExists("Student");
                    await _userManager.AddToRoleAsync(user, "Student");
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
        [Route("api/Student/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, RegisterDTO model)
        {
            try
            {
                var student = await _dtabaseSet.Students.FindAsync(id);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                student.fullName = model.fullName;
                student.Reg_no = model.Reg_no;
                student.Email = model.Email;
                student.Course = model.Course;
                student.Age = model.Age;

                _dtabaseSet.Students.Update(student);
                await _dtabaseSet.SaveChangesAsync();

                return StatusCode(200, "Student updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the student: " + ex.Message);
            }
        }


        [HttpGet]
        [Route("api/students")]

        public async Task<IActionResult> Lecturers()
        {
            var users = await _dtabaseSet.Students.ToListAsync();
            return Ok(users);
        }
    }
}

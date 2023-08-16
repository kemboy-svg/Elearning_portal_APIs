using Elearning__portal.Data;
using Elearning__portal.Models;
using Elearning__portal.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Elearning__portal.Controllers
{
    public class StudentController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DtabaseSet _dtabaseSet;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public StudentController(UserManager<Models.ApplicationUser> userManager, 
            DtabaseSet dtabaseSet,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _dtabaseSet = dtabaseSet;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
        [Route("api/StudentLogin")]
        public async Task<IActionResult> LecturerLogin([FromBody] LoginDTO model)
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

        [HttpPost]
        [Route("api/StudentRegister")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
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
                        Id = Guid.NewGuid(),
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
        [Route("api/Student/update{id}")]
        public async Task<IActionResult> UpdateStudent([FromBody] RegisterDTO model, Guid Id )
        {
            try
            {
                var student = await _dtabaseSet.Students.FindAsync(Id);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                student.fullName = model.fullName;
                student.Email = model.Email;
                student.Course = model.Course;
                student.Reg_no = model.Reg_no;
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

        public async Task<IActionResult> Students()
        {
            var users = await _dtabaseSet.Students.ToListAsync();
            return Ok(users);
        }

        [HttpDelete]
        [Route("api/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var student = await _dtabaseSet.Students.FindAsync(id);
                if(student == null)
                {
                    return NotFound("Students not found");
                }
                 _dtabaseSet.Students.Remove(student);
               await _dtabaseSet.SaveChangesAsync();
                return StatusCode(200, "Student deleted successfully");
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
     
        }
        [HttpGet]
        [Route("api/Notes")]

        public async Task<IActionResult> Notes()
        {
            var notes = await _dtabaseSet.Notes.ToListAsync();

            return Ok(notes);
        }


        [HttpPost]
        [Route("api/UnitEnrollment")]
        public async Task<IActionResult> RequestEnrollment([FromBody] EnrollmentRequestDTO request)
        {
            
            var existingEnrollment = await _dtabaseSet.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.Unit.unit_name == request.UnitName);

            if (existingEnrollment != null)
            {
                return BadRequest("Student is already enrolled in this unit.");
            }

            var unit = await _dtabaseSet.Units.FirstOrDefaultAsync(u => u.unit_name == request.UnitName);

            if (unit == null)
            {
                return NotFound();
            }

           
            var studentUnit = new Enrollment
            {
                StudentId = request.StudentId,
                UnitId = unit.Id,
                IsApproved = false
            };

            _dtabaseSet.Enrollments.Add(studentUnit);
            await _dtabaseSet.SaveChangesAsync();

            return Ok("Enrollment request submitted and awaiting approval");
        }


        [HttpGet]
        [Route("GetStudentByEmail")]
        public async Task<IActionResult> GetStudent(string email)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };

                var student = await _dtabaseSet.Students
                    .Include(e => e.Enrollments)
                    .ThenInclude(e => e.Unit)
                        .ThenInclude(n => n.Assignments)
                    .Include(e => e.Enrollments)
                    .ThenInclude(e => e.Unit)
                        .ThenInclude(n => n.Notes)
                    .FirstOrDefaultAsync(l => l.Email == email);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

               student.Enrollments = student.Enrollments.Where(e => e.IsApproved).ToList(); // Only approved enrollments

        foreach (var enrollment in student.Enrollments)
        {
            if (!enrollment.IsApproved)
            {
                enrollment.Unit = null;
            }
        }

        var serializedStudent = JsonSerializer.Serialize(student, options);

        return Ok(serializedStudent);
    }
    catch (Exception ex)
    {
        return StatusCode(500, "An error occurred while fetching the student details: " + ex.Message);
    }
}

    }
}

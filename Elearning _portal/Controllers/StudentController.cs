using Elearning__portal.Data;
using Elearning__portal.Models;
using Elearning__portal.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Azure.Core.HttpHeader;

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
                // Check if the user has a specific role (e.g., "Admin")
                if (await _userManager.IsInRoleAsync(user, "Student"))
                {
                    // Authentication successful and user has the required role
                    return StatusCode(200, "Logged in successfully as a student");
                }
                else
                {
                    return Unauthorized("You do not have permission to log in check your credentials and try again");
                }
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
        public async Task<IActionResult> Delete(Guid Id)
        {

            try
            {
                var student = await _dtabaseSet.Students.FindAsync(Id);
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
        [Route("view/Announcements")]

        public async Task<IActionResult> ViewAnnouncements()
        {
            var message=await _dtabaseSet.Announcements.ToListAsync();
            return Ok(message);
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

        [HttpGet] 
        [Route("api/viewAssignmentsAndNotesByUnitId")]
        public async Task<IActionResult> AssignmentsAndNotes(Guid unitId, string week)
        {
             
            if (string.IsNullOrEmpty(week))
            {
                return BadRequest("Week parameter is required.");
            }

            var assignments = await _dtabaseSet.Assignments
                .Where(a => a.UnitId == unitId && (a.Week == week || a.Week == null))
                .Select(assignment => new AssignmentAndNoteDTO
                {
                    AssignmentId = assignment.Id,
                    AssignmentDescription = assignment.Instruction,
                    AssignmentFileName = assignment.FileName,
                    AssignmentDueDate = assignment.DueDate,
                    AssignmentWeek=assignment.Week
                    
                })
                .ToListAsync();

            var notes = await _dtabaseSet.Notes
                .Where(n => n.UnitId == unitId && n.Week == week)
                .Select(note => new AssignmentAndNoteDTO
                {
                   
                    NoteId = note.Id,
                    NoteFileName = note.FileName,
                    NoteDescription = note.Description
                })
                .ToListAsync();

            var result = new
            {
                Assignments = assignments,
                Notes = notes
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("/api/DownloadAssignmentFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult DownloadFile(string fileName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "AssignmentUploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Set the Content-Disposition header to force the browser to download the file.
            var contentDisposition = new ContentDisposition
            {
                FileName = fileName,
                Inline = true,  // Set to true if you want the browser to try to open the file directly.
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(fileBytes, "application/octet-stream");
        }

        [HttpPost]
        [Route("api/SubmitAssignment")]
        public async Task<IActionResult> SubmitAssignment(AssignmentSubmisions model, Guid AssignmentId, Guid StudentId)
        {
            try
            {
               
                var existingSubmission = await _dtabaseSet.Submisions
                    .FirstOrDefaultAsync(s => s.AssignmentId == AssignmentId && s.StudentId == StudentId);

                if (existingSubmission != null)
                {
                    return BadRequest("You have already submitted this assignment.");
                }

            
                var assignment = await _dtabaseSet.Assignments.FindAsync(AssignmentId);
                if (assignment == null)
                {
                    return BadRequest("Assignment does not exist");
                }

                
                var submission = new AssignmentSubmisions
                {
                    AssignmentId = AssignmentId,
                    Content = model.Content,
                    StudentId = StudentId,
                    Week=model.Week,
                    Mark=0,
                    Remarks="Empty",
                    IsGraded=false
                };

                _dtabaseSet.Submisions.Add(submission);
                await _dtabaseSet.SaveChangesAsync();

                return Ok("Submitted Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred" + ex.Message);
            }
        }
        [HttpGet]
        [Route("api/getStudentSubmissions")]
        public async Task<IActionResult> GetSubmissions(Guid StudentId)
        {
            try
            {
                
                var submissions = await _dtabaseSet.Submisions
                    .Where(s => s.StudentId == StudentId)
                    .Include (e=>e.Assignment)
                    .ThenInclude(e=>e.Unit)
                    .ToListAsync();

                if (submissions == null || submissions.Count == 0)
                {
                    return BadRequest("No submissions found for the specified StudentId");
                }

                var result = submissions.Select(mySubmission=>new
                {
                    MarkAdded = mySubmission.Mark,
                    AddedRemarks=mySubmission.Remarks,
                    WeekDate=mySubmission.Week,
                    UnitName=mySubmission.Assignment.Unit.unit_name,
                    UnitCode=mySubmission.Assignment.Unit.unit_code,
                    Graded=mySubmission.IsGraded,

                });
               
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

       

        [HttpGet]
        [Route("api/GetAllAssignment")]
        public async Task<IActionResult> GetAssignments(string email)
        {
            try
            {
                var student = await _dtabaseSet.Students
                    .Include(e => e.Enrollments)
                    .ThenInclude(e => e.Unit)
                    .ThenInclude(n => n.Assignments)
                    .FirstOrDefaultAsync(l => l.Email == email);

                if (student == null)
                {
                    return NotFound("Student not found");
                }

                
                student.Enrollments = student.Enrollments.Where(e => e.IsApproved).ToList();

                
                var assignmentDTOs = new List<AssignmentDTO>();

                foreach (var enrollment in student.Enrollments)
                {
                    if (enrollment.Unit != null)
                    {

                        foreach (var assignment in enrollment.Unit.Assignments)
                        {
                            var isSubmitted = await _dtabaseSet.Submisions
                             .AnyAsync(s => s.AssignmentId == assignment.Id);

                            assignmentDTOs.Add(new AssignmentDTO
                            {
                               
                                AssignmentName = assignment.FileName,
                                DueDate = assignment.DueDate,
                                AssignmentDescription=assignment.Instruction,
                                UnitName=enrollment.Unit.unit_name,
                                Week=assignment.Week,
                                IsSubmitted=isSubmitted

                                
                            });
                        }
                    }
                }

                return Ok(assignmentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the student details: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("api/GetAnnouncements")]

        public async Task<IActionResult> Getnnouncements()
        {
            try
            {
                var announcements =await  _dtabaseSet.Announcements.ToListAsync();
                return Ok(announcements);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Failed to fetch"+ex.Message);
            }
        }



        [HttpPut]
        [Route("api/UploadProfile")]

        public async Task<IActionResult> UploadNotes(IFormFile file, [FromForm] Guid studentId, Student model)
        {
            try
            {
                var student = await _dtabaseSet.Students.FindAsync(studentId);
                
                
                if (student == null)
                {
                    return BadRequest("Anuothrized. The specified student does not exist.");
                }
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file selected");
                }
                var ProfileFolder = Path.Combine(Directory.GetCurrentDirectory(), "ProfileUploadsFolder");
                if (!Directory.Exists(ProfileFolder))
                    Directory.CreateDirectory(ProfileFolder);

                var profileName = Path.GetFileName(file.FileName);
                var FilePath = Path.Combine(ProfileFolder, profileName);

                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                
                student.ProfileName = profileName; 
                await _dtabaseSet.SaveChangesAsync();


                return Ok("Your profile uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred uploading your profile: " + ex.Message);
            }

        }



    }
}

﻿
using Elearning__portal.Data;
using Elearning__portal.Models;
using Elearning__portal.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elearning__portal.Controllers
{
    public class LecturerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DtabaseSet _dtabaseSet;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LecturerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DtabaseSet dtabaseSet,
            RoleManager<IdentityRole>roleManager,IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dtabaseSet = dtabaseSet;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment;
            
            
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
        [Route("api/LecturerLogin")]
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
                if (await _userManager.IsInRoleAsync(user, "Lecturer"))
                {
                    
                    return StatusCode(200, "Logged in successfully as Lecturer");
                }
                else
                {
                    return Unauthorized("You do not have permission to log in check your credentials and try again");
                }
            }

            return Unauthorized("Invalid login attempt");
        }




        [HttpPost]
        [Route("api/Register")]
        public async Task<IActionResult> Add([FromBody] RegisterDTO model)
        {
            try
            {
                var unit = await _dtabaseSet.Units.SingleOrDefaultAsync(u => u.unit_name == model.assigned_unit);
                if (unit == null)
                {
                    return BadRequest("Invalid unit, The specified unit does not exist or already assigned");
                }
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
                        
                        fullName = model.fullName,
                        Email = model.Email,
                        assigned_unit=unit.unit_name,
                        UnitId = unit.Id
                    };
                    

                    await _dtabaseSet.Lecturers.AddAsync(newL);
                    await _dtabaseSet.SaveChangesAsync();

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
        public async Task<IActionResult> UpdateLecturer(Guid Id, [FromBody] RegisterDTO model )
        {
            try
             {
                var lecturer = await _dtabaseSet.Lecturers.FindAsync(Id);

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

        [HttpGet]
        [Route("GetLecturerByEmail")]

        public async Task<IActionResult> Getlecturer(string email)
        {
            try {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                var lecturer = await _dtabaseSet.Lecturers
                .Include(l => l.Unit)
                .ThenInclude(u => u.Notes) 
                .Include(l => l.Unit)
                .ThenInclude(u => u.Assignments)
                .FirstOrDefaultAsync(l => l.Email == email);

                if (lecturer == null)
              {
                return NotFound("Lecturer not found");
              }
                var serializedLecturer = JsonSerializer.Serialize(lecturer, options);

                return Ok(serializedLecturer);

             }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the lecturer details: " + ex.Message);
            }


        }

        [HttpDelete]
        [Route("api/Lecturer/{id}")]
        public async Task<IActionResult> DeleteLecturer(Guid id)
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


        

        [HttpPost]
        [Route("api/UploadNotes")]

        public async Task<IActionResult> UploadNotes(IFormFile file, [FromForm] Guid unitId, Notes model)
        {
            try
            {
                var unit = await _dtabaseSet.Units.FindAsync(unitId);
                if (unit == null)
                {
                    return BadRequest("Invalid unitId. The specified unit does not exist.");
                }
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file selected");
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UnitNotes");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(file.FileName);
                var FilePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var fileDescription = new Notes
                {

                    FileName = fileName,
                    Description = model.Description,
                    Week = model.Week,
                    UnitId= unitId

                };
                _dtabaseSet.Notes.Add(fileDescription);
                await _dtabaseSet.SaveChangesAsync();


                return Ok("Notes uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred uploading the notes: " + ex.Message);
            }
            

           
            
            
        }

        [HttpPost]
        [Route("api/upload/Assignments")]

        public async Task<IActionResult> UploadAssignments (IFormFile file,  [FromForm] Guid unitId, Assignment model)
        {
           
            try
            { 
                var unit = await _dtabaseSet.Units.FindAsync(unitId);
                if (unit == null)
                {
                    return BadRequest("Invalid unitId. The specified unit does not exist.");
                }
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file selected");
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "AssignmentUploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var FileName = Path.GetFileName(file.FileName);
                var FilePath = Path.Combine(uploadsFolder, FileName);

                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var collection = new Assignment
                {
                    FileName = FileName,
                    Instruction = model.Instruction,
                    Week = model.Week,
                    DueDate = model.DueDate,
                    UnitId= unitId,

                };
                _dtabaseSet.Assignments.Add(collection);
                await _dtabaseSet.SaveChangesAsync();

                return StatusCode(200, "Assignment uploaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred uploading the assignment: " + ex.Message);
            }
           
        }

        [HttpGet]
        [Route("/api/DownloadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult DownloadFile(string fileName)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UnitNotes");
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



        [HttpGet]
        [Route("api/viewNotesByUnitId")]

        public async Task<IActionResult> Notes(Guid unitId)
        {
            var notes=await _dtabaseSet.Notes.Where
                (a => a.UnitId == unitId).ToListAsync();
            return Ok(notes);
        }

        [HttpGet]
        [Route("api/viewAssignmentsByUnitId")]
        public async Task<IActionResult> Assignments(Guid unitId)
      {
            var assignments = await _dtabaseSet.Assignments.Where
                (a => a.UnitId == unitId).ToListAsync();

            return Ok(assignments);
        }

        [HttpDelete]
        [Route("api/Delete/Assignment/{id}")]

        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            try
            {
                var assignment = await  _dtabaseSet.Assignments.FindAsync(id);
                if (assignment == null){
                    return NotFound("Assignment not found");
                }

                _dtabaseSet.Assignments.Remove(assignment);
                await _dtabaseSet.SaveChangesAsync();
                return Ok("Assignment removed successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while deleting the assignment"+ ex.Message);
            }
        }


        [HttpPut]
        [Route("api/ApproveStudent/{enrollmentId}")]
        public async Task<IActionResult> ApproveEnrollment(Guid enrollmentId)
        {
            
              
           var enrollment = await _dtabaseSet.Enrollments.FindAsync(enrollmentId);

            if (enrollment == null)
                {
                 return NotFound("Enrollment not found");
                 }

            //Approve the request here
            enrollment.IsApproved = true;
            await _dtabaseSet.SaveChangesAsync();


            // check on how to return the student details here
            return Ok("Enrollment approved and student details updated");
        }


        [HttpGet]
        [Route("GetStudentsWithPendingEnrollments")]
        public async Task<IActionResult> GetStudentsWithPendingEnrollments()
        {
            try
            {
                var notApprovedEnrollments = await _dtabaseSet.Enrollments
                    .Include(e => e.Unit)
                    .Where(e => !e.IsApproved)
                    .Include(e => e.Student)
                    .ToListAsync();

                var result = notApprovedEnrollments.Select(enrollment => new
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.Student.Id,
                    StudentName = enrollment.Student.fullName,
                    UnitId = enrollment.Unit.Id,
                    Reg_no = enrollment.Student.Reg_no,
                    email=enrollment.Student.Email,
                    UnitName = enrollment.Unit.unit_name,
                    IsApproved = enrollment.IsApproved
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return appropriate error response
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request."+ex.Message);
            }
        }




        [HttpGet]
        [Route("GetStudentsWithAprovedEnrollments")]
        public async Task<IActionResult> ApprovedStudents()
        {
            try
            {
                var approvedEnrollments = await _dtabaseSet.Enrollments
                .Include(e => e.Unit)
                .Where(e => e.IsApproved)
                .Include(e => e.Student)
                .ToListAsync();

                var result = approvedEnrollments.Select(enrollment => new
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = enrollment.Student.Id,
                    StudentName = enrollment.Student.fullName,
                    UnitId = enrollment.Unit.Id,
                    Reg_no = enrollment.Student.Reg_no,
                    email = enrollment.Student.Email,
                    UnitName = enrollment.Unit.unit_name,
                    IsApproved = enrollment.IsApproved
                });

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request." + ex.Message);

            }
            
        }



        //Api to remove enrollment reuest whether approved
        //Rejects the request 
        [HttpDelete]
        [Route("api/RemoveEnrollment/{enrollmentId}")]
        public async Task<IActionResult> RemoveEnrollment(Guid enrollmentId)
        {
            var enrollment = await _dtabaseSet.Enrollments.FindAsync(enrollmentId);

            if (enrollment == null)
            {
                return NotFound("Enrollment not found");
            }

            _dtabaseSet.Enrollments.Remove(enrollment);
            await _dtabaseSet.SaveChangesAsync();

            return Ok("Enrollment removed");
        }


        [HttpPost]
        [Route("post/Announcements")]

        public async Task<IActionResult> PostAnnouncements(Guid lecturerId, Announcements model)
        {
            try
            {
                var lecturer = await _dtabaseSet.Lecturers.FindAsync(lecturerId);
                if (lecturer== null)
                {
                    return BadRequest("Sender unable to complete the request");
                }

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

        //[HttpGet]
        //[Route("api/getSubmissionsById")]

        //public async Task<IActionResult> GetSubmissions(Guid Id)
        //{
        //    try
        //    {
        //        var submissionsWithStudents = await _dtabaseSet.Submisions
        //            .Where(a => a.AssignmentId == Id)
        //            .Include(a => a.Student) // Include the Student navigation property
        //            .ToListAsync();

        //        // Group submissions by StudentId
        //        var groupedSubmissions = submissionsWithStudents.GroupBy(s => s.StudentId);

        //        var result = groupedSubmissions.Select(group =>
        //        {
        //            var student = group.First().Student; // Since all submissions have the same student, just take the first one

        //            return new
        //            {
        //                StudentId = student.Id,
        //                StudentName = student.fullName,
        //                RegNo=student.Reg_no,
        //                Submissions = group.Select(submission => new
        //                {
        //                    SubmissionId = submission.Id,
        //                    SubmissionDate = submission.SubmissionTime,
        //                    SubmissionContent=submission.Content
        //                    // Add other submission properties as needed
        //                }).ToList()
        //            };
        //        }).ToList();

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Some error occurred: " + ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("api/getSubmissionsById")]

        public async Task<IActionResult>GetSubmissions(Guid Id)
        {
            try
            {
                var submissions = await _dtabaseSet.Submisions
                    .Where(a => a.AssignmentId == Id)
                    .Include(a => a.Student) 
                    .ToListAsync();
                return Ok(submissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while fetching" + ex.Message);
               
            }
        }

        [HttpPut]
        [Route("api/updateMarks")]
        public async Task<IActionResult> UpdateMarks(Guid Id, AssignmentSubmisions model)
        {
            try
            {
                var submission = await _dtabaseSet.Submisions.FindAsync(Id);
                if (submission == null)
                {
                    return BadRequest("Submission not found");
                }

                submission.Mark = model.Mark;
                submission.Remarks = model.Remarks;
                submission.IsGraded = true;

                await _dtabaseSet.SaveChangesAsync();

                return Ok("Marks updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while updating: " + ex.Message);
            }
        }



    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  Guid Id { get; set; }
        public string Reg_no { get; set; }
        public string Email { get; set; }
        public string Course  { get; set; }
        public string fullName { get; set; }
        public int Age { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }

    }
}

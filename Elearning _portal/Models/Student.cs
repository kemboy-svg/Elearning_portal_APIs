using Microsoft.AspNetCore.Identity;

namespace Elearning__portal.Models
{
    public class Student
    {
        public  int Id { get; set; }
        public string Reg_no { get; set; }
        public string Email { get; set; }
        public string Course  { get; set; }
        public string fullName { get; set; }
        public int Age { get; set; }

    }
}

using Microsoft.AspNetCore.Identity;

namespace Elearning__portal.Models
{
    
    public class Lecturer
    {
        public int Id { get; set; }
        public string fullName { get; set; }
        public string Email { get; set; }
        public string assigned_unit { get; set; }


    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    
    public class Lecturer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string fullName { get; set; }
        public string Email { get; set; }
        public string assigned_unit { get; set; }

        
        public Guid UnitId { get; set; }
        public virtual Unit Unit { get; set; }


    }
}

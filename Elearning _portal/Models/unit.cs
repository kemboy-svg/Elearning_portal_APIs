using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int unit_code { get; set; }
        public string unit_name { get; set; }


       
        public ICollection<Student> Students { get; set; }

        public ICollection<Notes> Notes { get; set; }
        public ICollection<Assignment> Assignments { get; set; }

    }
}

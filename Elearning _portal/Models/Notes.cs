using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class Notes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set;}
        public string Description { get; set;}
        public string Week { get; set;}

        public DateTime CreatedAt { get; set;}

        public Notes() 
        { 
            CreatedAt = DateTime.Now;
        }
    }
}

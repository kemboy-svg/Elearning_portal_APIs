using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class Uploads
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set;}
        public string Description { get; set;}

        public DateTime CreatedAt { get; set;}

        public Uploads() 
        { 
            CreatedAt = DateTime.Now;
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class Notes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FileName { get; set;}
        public string Description { get; set;}
        public string Week { get; set;}

        public DateTime CreatedAt { get; set;}

        public virtual Unit Unit { get; set; }
        public Guid UnitId { get; set; }

        public Notes() 
        { 
            CreatedAt = DateTime.Now;
        }
    }
}

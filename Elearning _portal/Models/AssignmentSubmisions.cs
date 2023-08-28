using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class AssignmentSubmisions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Content { get; set; }
        public DateTime SubmissionTime { get; set; }

        public int Mark { get; set; }

        public string Remarks { get; set; }
        public string Week { get; set; }
        public bool IsGraded { get; set; }
        public Guid StudentId { get; set; }
        
        public virtual Student Student { get; set; }

        public Guid AssignmentId { get; set; }
        
        public virtual Assignment Assignment { get; set; }
        public AssignmentSubmisions()
        {
            SubmissionTime=DateTime.Now;
        }
            

    }
}

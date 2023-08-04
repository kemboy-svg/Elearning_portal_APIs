

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elearning__portal.Models
{
    public class Assignment
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Instruction { get; set; }

        public string Week { get; set; }
        public  DateTime DueDate { get; set; }
        
        public DateTime CreateAt { get; set; }

        public Guid UnitId { get; set; }
        public virtual Unit Unit { get; set; }
       


        public Assignment()
        {
            CreateAt = DateTime.Now;
        }
    }
}

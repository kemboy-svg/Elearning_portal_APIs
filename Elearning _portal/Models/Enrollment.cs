﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Elearning__portal.Models
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public Guid UnitId { get; set; }
        public Unit Unit { get; set; }

        public bool IsApproved { get; set; }
    }
}

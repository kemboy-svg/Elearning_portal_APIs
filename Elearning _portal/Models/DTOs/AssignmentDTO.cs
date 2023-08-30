namespace Elearning__portal.Models.DTOs
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; }
        public DateTime DueDate { get; set; }
        public string UnitName { get; set; }
        public string AssignmentDescription { get; set; }
        public string Week { get; set; }
        public bool IsSubmitted { get; set; }

    }
}

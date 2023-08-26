namespace Elearning__portal.Models.DTOs
{
    public class AssignmentAndNoteDTO
    {
        public Guid AssignmentId { get; set; }
        public string AssignmentDescription { get; set; }

        public string AssignmentFileName { get; set; }
        public DateTime AssignmentDueDate { get; set; }
        public string AssignmentWeek { get; set; }
        public Guid NoteId { get; set; }
        public string NoteDescription { get; set; }
        public string NoteFileName { get; set; }
    }
}

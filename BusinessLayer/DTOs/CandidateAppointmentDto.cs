namespace BusinessLayer.DTOs
{
    public class CandidateAppointmentDto
    {
        public int InterviewId { get; set; }
        public int CandidateId { get; set; }

        public string SeqNo { get; set; } = null!;
        public DateTime InterviewDate { get; set; }
        public string? Designation { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
    }
}

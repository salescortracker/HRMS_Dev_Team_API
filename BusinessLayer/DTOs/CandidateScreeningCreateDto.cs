namespace BusinessLayer.DTOs
{
    public class CandidateScreeningCreateDto
    {
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public List<int> CandidateIds { get; set; } = new();
        public List<int> RecruiterIds { get; set; } = new();
        public string Result { get; set; } = null!;
        public string? Remarks { get; set; }
    }
}

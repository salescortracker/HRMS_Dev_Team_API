using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ResumeParseResultDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Skills { get; set; }
        public string? Location { get; set; }
        public string? Designation { get; set; }

        public List<CandidateExperienceDto> Experiences { get; set; } = new();
        public List<CandidateQualificationDto> Qualifications { get; set; } = new();
    }
}

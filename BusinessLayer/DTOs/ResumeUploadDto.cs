using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class ResumeUploadDto
    {
        public IFormFile ResumeFile { get; set; }
    }
}

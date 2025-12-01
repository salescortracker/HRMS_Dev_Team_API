

namespace BusinessLayer.DTOs
{
    public class DocumentTypeDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}

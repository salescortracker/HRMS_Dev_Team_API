using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{



    // Read model 
    public class BulkTicketApprovalDto
    {
        public List<int> RaiseTicketApprovalIds { get; set; } = new();
        public string? ManagerComments { get; set; }
        public int ManagerId { get; set; }
        public string? TicketStatus { get; set; }

        public int ActionBy { get; set; }
    }

    public class RaiseTicketApprovalUpdateDto
    {
        public int RaiseTicketApprovalId { get; set; }
        public int RaiseTicketId { get; set; }
        public int? ManagerStatus { get; set; }
        public string? ManagerComments { get; set; }
        public int ManagerId { get; set; }
        public string? TicketStatus { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

}
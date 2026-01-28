using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class StateDto
    {
        public int StateId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
    }
}
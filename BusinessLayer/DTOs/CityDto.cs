using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CityDto
    {
        public int CityId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int StateId { get; set; }
        public string CityName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ResignationTypeDto
    {
        public int ResignationTypeId { get; set; }
        public string ResignationTypeName { get; set; }
        public int NoticePeriod { get; set; }
    }
}

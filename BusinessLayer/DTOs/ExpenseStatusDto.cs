using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ExpenseStatusDto
    {
        public int ExpenseStatusID { get; set; }
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
        public string ExpenseStatusName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

    }
}

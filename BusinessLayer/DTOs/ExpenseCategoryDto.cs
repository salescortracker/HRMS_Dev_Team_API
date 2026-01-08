using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class ExpenseCategoryDto
    {
        public int ExpenseCategoryID { get; set; }
        public string ExpenseCategoryName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int RegionID { get; set; }
    }
}

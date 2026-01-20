using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class TaxTypeDto
    {
        public int TaxTypeId { get; set; }
        public string TaxTypeName { get; set; } = null!;
    }
}

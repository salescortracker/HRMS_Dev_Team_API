using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class TaxSettingsDto
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; } = null!;
        public int TaxTypeId { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
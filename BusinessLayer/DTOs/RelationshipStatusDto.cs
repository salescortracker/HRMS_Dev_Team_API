using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{

    public class RelationshipDto
    {
        public int relationshipId { get; set; }
        public int companyId { get; set; }
        public int regionId { get; set; }
        public string relationshipName { get; set; } = null!;
        public bool isActive { get; set; }
    }
}
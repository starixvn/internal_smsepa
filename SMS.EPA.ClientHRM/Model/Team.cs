using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.Model
{
    public class Team
    {
        public int TeamId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public int LeaderId { get; set; }

        public int AssistantLeaderId { get; set; }

        public int Rank { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

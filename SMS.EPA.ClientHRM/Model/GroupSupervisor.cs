using System;

namespace SMS.EPA.ClientHRM.Model
{
    public class GroupSupervisor
    {
        public int TeamId { get; set; }

        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

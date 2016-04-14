using System;

namespace SMS.EPA.ServicesAPI.Model
{
    public class TeamDisciplinePermission
    {
        public int TeamId { get; set; }

        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

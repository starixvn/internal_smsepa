namespace SMS.EPA.ServicesAPI.Model
{
    using System;

    public class TeamDisciplinePermission
    {
        public int TeamId { get; set; }

        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

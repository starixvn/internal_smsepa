namespace SMS.EPA.ClientHRM.Model
{
    using System;

    public class GroupDisciplinePermission
    {
        public int TeamId { get; set; }

        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

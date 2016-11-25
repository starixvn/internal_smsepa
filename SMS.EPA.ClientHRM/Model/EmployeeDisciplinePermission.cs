namespace SMS.EPA.ClientHrm.Model
{
    using System;

    public class EmployeeDisciplinePermission
    {
        public int EmployeeId { get; set; }

        public int OwnerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
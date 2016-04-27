namespace SMS.EPA.ClientHRM.Model
{
    using System;

    public class Department
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int ManagerId { get; set; }

        public int DepartmentLevel { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}

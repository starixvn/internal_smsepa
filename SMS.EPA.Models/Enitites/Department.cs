namespace SMS.EPA.Model.Enitites
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Department
    {
        public Department()
        {
            Childrens = new List<Department>();
        }

        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public bool IsActive { get; set; }

        public bool IsGroup { get; set; }

        public Employee Manager { get; set; }

        public List<Department> Childrens { get; set; }
    }
}
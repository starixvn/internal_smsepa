namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using Enitites;

    public class DepartmentInfoModel
    {
        public DepartmentInfoModel()
        {
            Departments = new List<Department>();
        }

        public Department TreeDepartment { get; set; }

        public IList<Department> Departments { get; set; }
    }
}
namespace SMS.EPA.Model
{
    using Enitites;
    using System.Collections.Generic;

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
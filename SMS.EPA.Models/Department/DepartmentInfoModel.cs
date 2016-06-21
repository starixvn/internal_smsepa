namespace SMS.EPA.Model.Department
{
    using System.Collections.Generic;

    public class DepartmentInfoModel
    {
        public DepartmentInfoModel()
        {
            Departments = new List<DepartmentModel>();
        }

        public DepartmentModel TreeDepartment { get; set; }

        public IList<DepartmentModel> Departments { get; set; }
    }
}
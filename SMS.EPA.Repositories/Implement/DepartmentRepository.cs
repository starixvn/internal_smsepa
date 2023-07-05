namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.Enitites;

    public class DepartmentRepository : IDepartmentRepository
    {
        public IEnumerable<Department> GetDepartment(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                IEnumerable<Department> departments = db.Query<Department, Employee, Department>(
                   (dept, manager) =>
                   {
                       dept.Manager = manager;

                       return dept;
                   },
                   "ManagerId",
                   criteria);

                return departments;
            }
        }
    }
}
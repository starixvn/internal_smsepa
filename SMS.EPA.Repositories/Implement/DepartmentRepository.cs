namespace SMS.EPA.Repositories.Implement
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;
    using System.Collections.Generic;

    public class DepartmentRepository : IDepartmentRepository
    {
        public IEnumerable<Department> GetDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IEnumerable<Department> departments = db.Query<Department, Employee, Department>(
                   (dept, manager) =>
                   {
                       dept.Manager = manager;

                       return dept;
                   },
                   "ManagerId",
                   criteria.GetSpParams());

                return departments;
            }
        }
    }
}
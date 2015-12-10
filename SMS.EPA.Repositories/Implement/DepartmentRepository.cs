// -----------------------------------------------------------------------
// <copyright file="DepartmentRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.21">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class DepartmentRepository : IDepartmentRepository
    {
        public List<DepartmentModel> GetDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                List<DepartmentModel> departments = db.Query<DepartmentModel, Employee, DepartmentModel>(
                   (dept, manager) =>
                   {
                       dept.Manager = manager;

                       return dept;
                   },
                   "ManagerId",
                   criteria.GetSpParams()).ToList();

                if (departments.Any())
                {
                    departments.ForEach(x => x.Childrens.AddRange(departments.Where(y => y.ParentId == x.DepartmentId)));
                }

                return departments;
            }
        }
    }
}
// -----------------------------------------------------------------------
// <copyright file="EmployeeEPARepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EmployeeEPAModel> GetEPAByEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<EmployeeEPAModel, DepartmentModel, Group, StatusModel, EmployeeModel, EmployeeEPAModel>(
                    (me, dept, group, status, ev) =>
                    {
                        me.Department = dept;
                        me.Group = group;
                        me.Status = status;
                        me.Evaluator = ev;
                        return me;
                    },
                    "DepartmentId, GroupId, StatusId, EvaluatorId",
                    criteria.GetSpParams());
            }
        }
    }
}
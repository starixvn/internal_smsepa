// ----------------------------------------------------------------------
// <copyright file="AttendanceSummaryRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/09/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class AttendanceSummaryRepository : IAttendanceSummaryRepository
    {
        public RawAttendanceSummaryModel GetAttendanceSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, Group, EmployeeModel>(
                        (emp, gr) =>
                        {
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId");

                    IEnumerable<AttendanceSummaryModel> employeeViolations = multi.Read<AttendanceSummaryModel>();

                    var attendanceSummary = new RawAttendanceSummaryModel
                    {
                        Employees = employees,
                        EmployeeViolations = employeeViolations
                    };
                    return attendanceSummary;
                }
            }
        }
    }
}

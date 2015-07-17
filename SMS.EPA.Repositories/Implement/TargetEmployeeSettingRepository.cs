// ----------------------------------------------------------------------
// <copyright file="TargetEmployeeSettingRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Interfaces;

    public class TargetEmployeeSettingRepository : ITargetEmployeeSettingRepository
    {
        public RawTargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria)
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

                    IEnumerable<TargetEmployeeModel> targetEmployees = multi.Read<TargetEmployeeModel, TargetTemplateModel, TargetEmployeeModel>(
                        (tarEmp, tar) =>
                        {
                            tarEmp.TargetTemplate = tar;
                            return tarEmp;
                        },
                        "TargetTemplateId");

                    var targetEmployeeSetting = new RawTargetEmployeeSettingModel
                    {
                        Employees = employees,
                        TargetEmployees = targetEmployees
                    };

                    return targetEmployeeSetting;
                }
            }
        }
    }
}

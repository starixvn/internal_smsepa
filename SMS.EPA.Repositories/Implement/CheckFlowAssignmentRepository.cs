// ----------------------------------------------------------------------
// <copyright file="CheckFlowAssignmentRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.CheckFlowAssignment;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Interfaces;

    public class CheckFlowAssignmentRepository : ICheckFlowAssignmentRepository
    {
        public RawEmployeeFlowTargetSettingModel GetEmployeeFlowTargetSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeFlowTargetSettingModel> employees = multi.Read<EmployeeFlowTargetSettingModel, Group, JobTitle, DepartmentModel, EmployeeFlowTargetSettingModel>(
                        (emp, gr, job, dept) =>
                        {
                            emp.Group = gr;
                            emp.JobTitle = job;
                            emp.Department = dept;
                            return emp;
                        },
                        "GroupId, JobTitleId, DepartmentId");

                    IEnumerable<TargetEmployeeModel> targetEmployees = multi.Read<TargetEmployeeModel, TargetTemplateModel, TargetEmployeeModel>(
                        (tarEmp, tar) =>
                        {
                            tarEmp.TargetTemplate = tar;
                            return tarEmp;
                        },
                        "TargetTemplateId");

                    IEnumerable<RawEmployeeFlowModel> employeeFlows = multi.Read<RawEmployeeFlowModel, EmployeeModel, RawEmployeeFlowModel>(
                        (empFlow, eva) =>
                        {
                            empFlow.EvaluatorInfo = eva;
                            return empFlow;
                        },
                        "EvaluatorId");

                    var employeeFlowTargetSetting = new RawEmployeeFlowTargetSettingModel
                    {
                        Employees = employees,
                        TargetEmployees = targetEmployees,
                        EmployeeFlows = employeeFlows
                    };

                    return employeeFlowTargetSetting;
                }
            }
        }
    }
}

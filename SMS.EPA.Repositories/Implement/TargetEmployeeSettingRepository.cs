namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Repositories.Interfaces;

    public class TargetEmployeeSettingRepository : ITargetEmployeeSettingRepository
    {
        public TargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
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
                            tarEmp.RawTargetTemplate = tar;
                            return tarEmp;
                        },
                        "TargetTemplateId");

                    var targetEmployeeSetting = new TargetEmployeeSettingModel
                    {
                        Employees = employees,
                        TargetEmployees = targetEmployees
                    };

                    return targetEmployeeSetting;
                }
            }
        }

        public TargetDepartmentSettingModel GetTargetDepartmentSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<DepartmentModel> deparments = multi.Read<DepartmentModel>();

                    IEnumerable<TargetDepartmentModel> targetDepartments = multi.Read<TargetDepartmentModel, TargetTemplateModel, TargetDepartmentModel>(
                        (tarEmp, tar) =>
                        {
                            tarEmp.RawTargetTemplate = tar;
                            return tarEmp;
                        },
                        "TargetTemplateId");

                    var targetDepartmentSetting = new TargetDepartmentSettingModel
                    {
                        Departments = deparments,
                        TargetDepartments = targetDepartments
                    };

                    return targetDepartmentSetting;
                }
            }
        }
    }
}
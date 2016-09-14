namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Repositories.Interfaces;
    using System.Linq;

    public class TargetEmployeeSettingRepository : ITargetEmployeeSettingRepository
    {
        public TargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<Employee> employees = multi.Read<Employee, Group, Employee>(
                        (emp, gr) =>
                        {
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId");

                    IEnumerable<TargetEmployeeModel> targetEmployees = multi.Read<TargetEmployeeModel, TargetTemplate, TargetEmployeeModel>(
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
                    IEnumerable<Department> deparments = multi.Read<Department>();

                    IEnumerable<TargetDepartmentModel> targetDepartments = multi.Read<TargetDepartmentModel, TargetTemplate, TargetDepartmentModel>(
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
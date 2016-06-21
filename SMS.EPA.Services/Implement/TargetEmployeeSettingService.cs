namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class TargetEmployeeSettingService : BaseService, ITargetEmployeeSettingService
    {
        private readonly ITargetEmployeeSettingRepository _targetEmployeeSettingRepository;

        public TargetEmployeeSettingService()
            : this(null)
        {
        }

        public TargetEmployeeSettingService(ITargetEmployeeSettingRepository targetEmployeeSettingRepository)
        {
            _targetEmployeeSettingRepository = targetEmployeeSettingRepository ?? new TargetEmployeeSettingRepository();
        }

        public IList<TargetEmployeeModel> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria)
        {
            var targetEmployeeSettings = new List<TargetEmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                TargetEmployeeSettingModel rawEmployeeSetting = _targetEmployeeSettingRepository.GetTargetEmployeeSetting(criteria);
                targetEmployeeSettings = ConvertToTargetEmployeeModel(rawEmployeeSetting);
            }

            return targetEmployeeSettings;
        }

        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteTargetEmployeeSetting(DeleteTargetEmployeeSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IList<TargetDepartmentModel> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria)
        {
            var targetDepartmentSettings = new List<TargetDepartmentModel>();

            if (criteria != null && criteria.IsValid())
            {
                TargetDepartmentSettingModel rawDepartmentSeting = _targetEmployeeSettingRepository.GetTargetDepartmentSetting(criteria);
                targetDepartmentSettings = ConvertToTargetDepartmentModel(rawDepartmentSeting);
            }

            return targetDepartmentSettings;
        }

        public CommandResult AssignTargetTemplateToDepartment(AssignTargetDepartmentSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteTargetDepartmentSetting(DeleteTargetDepartmentSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        private List<TargetEmployeeModel> ConvertToTargetEmployeeModel(TargetEmployeeSettingModel employeeSetting)
        {
            var targetEmployeeSettings = new List<TargetEmployeeModel>();

            if (employeeSetting != null)
            {
                foreach (EmployeeModel employee in employeeSetting.Employees)
                {
                    var targetEmployeeSetting = new TargetEmployeeModel
                    {
                        Employee = employee,
                        Group = employee.Group
                    };

                    if (employeeSetting.TargetEmployees != null)
                    {
                        IEnumerable<TargetEmployeeModel> targetEmployees = employeeSetting.TargetEmployees.Where(x => x.EmployeeId == employee.EmployeeId);

                        if (targetEmployees != null && targetEmployees.Any())
                        {
                            targetEmployeeSetting.TargetTemplates = GetTargetTemplateOfEmployee(targetEmployees);
                        }
                    }

                    targetEmployeeSettings.Add(targetEmployeeSetting);
                }
            }

            return targetEmployeeSettings;
        }

        private IList<TargetTemplateModel> GetTargetTemplateOfEmployee(IEnumerable<TargetEmployeeModel> targetEmployees)
        {
            IList<TargetTemplateModel> targetTemplates = targetEmployees.Select(x => new TargetTemplateModel
            {
                TargetTemplateId = x.RawTargetTemplate.TargetTemplateId,
                Name = x.RawTargetTemplate.Name,
                TargetTypeId = x.RawTargetTemplate.TargetTypeId,
                TargetTypeName = x.RawTargetTemplate.TargetTypeName
            }).ToList();

            return targetTemplates;
        }

        private List<TargetDepartmentModel> ConvertToTargetDepartmentModel(TargetDepartmentSettingModel departmentSeting)
        {
            var targetDepartmentSettings = new List<TargetDepartmentModel>();

            if (departmentSeting != null)
            {
                foreach (DepartmentModel department in departmentSeting.Departments)
                {
                    var targetDepartmentSetting = new TargetDepartmentModel
                    {
                        DepartmentId = department.DepartmentId,
                        Name = department.Name,
                        ParentId = department.ParentId,
                        IsActive = department.IsActive,
                        IsGroup = department.IsGroup
                    };

                    if (departmentSeting.TargetDepartments != null)
                    {
                        IEnumerable<TargetDepartmentModel> rawTargetDepartments = departmentSeting.TargetDepartments
                                                    .Where(x => x.DepartmentId == department.DepartmentId && x.IsGroup == department.IsGroup);

                        if (rawTargetDepartments != null && rawTargetDepartments.Any())
                        {
                            targetDepartmentSetting.TargetTemplates = GetTargetTemplateOfDepartment(rawTargetDepartments);
                        }
                    }

                    targetDepartmentSettings.Add(targetDepartmentSetting);
                }
            }

            return targetDepartmentSettings;
        }

        private IList<TargetTemplateModel> GetTargetTemplateOfDepartment(IEnumerable<TargetDepartmentModel> targetDepartments)
        {
            return targetDepartments.Select(x => new TargetTemplateModel
            {
                TargetTemplateId = x.RawTargetTemplate.TargetTemplateId,
                Name = x.RawTargetTemplate.Name,
                TargetTypeId = x.RawTargetTemplate.TargetTypeId,
                TargetTypeName = x.RawTargetTemplate.TargetTypeName
            }).ToList();
        }
    }
}
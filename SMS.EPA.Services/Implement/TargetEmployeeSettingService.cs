// ----------------------------------------------------------------------
// <copyright file="TargetEmployeeSettingService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/20/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
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
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                RawTargetEmployeeSettingModel rawEmployeeSetting = _targetEmployeeSettingRepository.GetTargetEmployeeSetting(criteria);
                targetEmployeeSettings = ConvertToTargetEmployeeModel(rawEmployeeSetting);
            }

            return targetEmployeeSettings;
        }

        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteTargetEmployeeSetting(DeleteTargetEmployeeSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IList<TargetDepartmentModel> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria)
        {
            var targetDepartmentSettings = new List<TargetDepartmentModel>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                RawTargetDepartmentSettingModel rawDepartmentSeting = _targetEmployeeSettingRepository.GetTargetDepartmentSetting(criteria);
                targetDepartmentSettings = ConvertToTargetDepartmentModel(rawDepartmentSeting);
            }

            return targetDepartmentSettings;
        }

        public CommandResult AssignTargetTemplateToDepartment(AssignTargetDepartmentSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteTargetDepartmentSetting(DeleteTargetDepartmentSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        private List<TargetEmployeeModel> ConvertToTargetEmployeeModel(RawTargetEmployeeSettingModel rawEmployeeSetting)
        {
            var targetEmployeeSettings = new List<TargetEmployeeModel>();

            if (rawEmployeeSetting != null)
            {
                foreach (EmployeeModel employee in rawEmployeeSetting.Employees)
                {
                    var targetEmployeeSetting = new TargetEmployeeModel
                    {
                        EmployeeId = employee.EmployeeId,
                        Code = employee.Code,
                        NickName = employee.NickName,
                        FullName = employee.FullName,
                        Enabled = employee.Enabled,
                        Group = employee.Group
                    };

                    if (rawEmployeeSetting.TargetEmployees != null)
                    {
                        IEnumerable<TargetEmployeeModel> rawTargetEmployees = rawEmployeeSetting.TargetEmployees.Where(x => x.EmployeeId == employee.EmployeeId);

                        if (rawTargetEmployees != null && rawTargetEmployees.Count() > 0)
                        {
                            targetEmployeeSetting.TargetTemplates = GetTargetTemplateOfEmployee(rawTargetEmployees);
                        }
                    }

                    targetEmployeeSettings.Add(targetEmployeeSetting);
                }
            }

            return targetEmployeeSettings;
        }

        private IList<TargetTemplateModel> GetTargetTemplateOfEmployee(IEnumerable<TargetEmployeeModel> rawTargetEmployees)
        {
            IList<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            foreach (TargetEmployeeModel rawTargetEmployee in rawTargetEmployees)
            {
                var targetTemplate = new TargetTemplateModel
                {
                    TargetTemplateId = rawTargetEmployee.RawTargetTemplate.TargetTemplateId,
                    Name = rawTargetEmployee.RawTargetTemplate.Name,
                    TargetTypeId = rawTargetEmployee.RawTargetTemplate.TargetTypeId,
                    TargetTypeName = rawTargetEmployee.RawTargetTemplate.TargetTypeName
                };

                targetTemplates.Add(targetTemplate);
            }

            return targetTemplates;
        }

        private List<TargetDepartmentModel> ConvertToTargetDepartmentModel(RawTargetDepartmentSettingModel rawDepartmentSeting)
        {
            var targetDepartmentSettings = new List<TargetDepartmentModel>();

            if (rawDepartmentSeting != null)
            {
                foreach (DepartmentModel department in rawDepartmentSeting.Departments)
                {
                    var targetDepartmentSetting = new TargetDepartmentModel
                    {
                        DepartmentId = department.DepartmentId,
                        Name = department.Name,
                        ParentId = department.ParentId,
                        IsActive = department.IsActive,
                        IsGroup = department.IsGroup
                    };

                    if (rawDepartmentSeting.TargetDepartments != null)
                    {
                        IEnumerable<TargetDepartmentModel> rawTargetDepartments = rawDepartmentSeting.TargetDepartments
                                                    .Where(x => x.DepartmentId == department.DepartmentId && x.IsGroup == department.IsGroup);

                        if (rawTargetDepartments != null && rawTargetDepartments.Count() > 0)
                        {
                            targetDepartmentSetting.TargetTemplates = GetTargetTemplateOfDepartment(rawTargetDepartments);
                        }
                    }

                    targetDepartmentSettings.Add(targetDepartmentSetting);
                }
            }

            return targetDepartmentSettings;
        }

        private IList<TargetTemplateModel> GetTargetTemplateOfDepartment(IEnumerable<TargetDepartmentModel> rawTargetDepartments)
        {
            IList<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            foreach (TargetDepartmentModel rawTargetDepartment in rawTargetDepartments)
            {
                var targetTemplate = new TargetTemplateModel
                {
                    TargetTemplateId = rawTargetDepartment.RawTargetTemplate.TargetTemplateId,
                    Name = rawTargetDepartment.RawTargetTemplate.Name,
                    TargetTypeId = rawTargetDepartment.RawTargetTemplate.TargetTypeId,
                    TargetTypeName = rawTargetDepartment.RawTargetTemplate.TargetTypeName
                };

                targetTemplates.Add(targetTemplate);
            }

            return targetTemplates;
        }
    }
}
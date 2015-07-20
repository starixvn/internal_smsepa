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

        private List<TargetEmployeeModel> ConvertToTargetEmployeeModel(RawTargetEmployeeSettingModel rawEmployeeSetting)
        {
            var targetEmployeeSettings = new List<TargetEmployeeModel>();

            foreach (EmployeeModel employee in rawEmployeeSetting.Employees)
            {
                var targetEmployeeSetting = new TargetEmployeeModel
                {
                    EmployeeId = employee.EmployeeId,
                    Code = employee.Code,
                    NickName = employee.NickName,
                    FullName = employee.FullName,
                    Enabled = employee.Enabled,
                    Group = employee.Group,
                    TargetTemplate = new TargetTemplateModel()
                };

                if (rawEmployeeSetting.TargetEmployees != null)
                {
                    IEnumerable<TargetEmployeeModel> rawTargetEmployees = rawEmployeeSetting.TargetEmployees.Where(x => x.EmployeeId == employee.EmployeeId);

                    if (rawTargetEmployees != null && rawTargetEmployees.Count() > 0)
                    {
                        targetEmployeeSetting.TargetTemplate = rawTargetEmployees.First().TargetTemplate;
                    }
                }

                targetEmployeeSettings.Add(targetEmployeeSetting);
            }

            return targetEmployeeSettings;
        }
    }
}

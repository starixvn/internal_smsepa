// ----------------------------------------------------------------------
// <copyright file="TargetEmployeeSettingController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/20/2015">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class TargetEmployeeSettingController : BaseController
    {
        private readonly ITargetEmployeeSettingService _targetEmployeeSettingService;

        public TargetEmployeeSettingController()
            : this(null)
        {
        }

        public TargetEmployeeSettingController(ITargetEmployeeSettingService targetEmployeeSettingService)
        {
            _targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
        }

        public IList<TargetEmployeeModel> GetTargetEmployeeSetting(
            int? groupId = null,
            int? employeeId = null,
            int? targetTypeId = null,
            bool? showAll = null)
        {
            var criteria = new GetAllTargetEmployeeSettingCriteria
            {
                GroupId = groupId,
                EmployeeId = employeeId,
                TargetTypeId = targetTypeId,
                ShowAll = showAll
            };

            IList<TargetEmployeeModel> targetEmployeeSettings = _targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);
            return targetEmployeeSettings;
        }

        [HttpPut]
        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _targetEmployeeSettingService.AssignTargetTemplateToEmployee(criteria);
            return commandResult;
        }

        [HttpDelete]
        public CommandResult DeleteTargetEmployeeSetting(int targetTemplateId, int employeeId)
        {
            var criteria = new DeleteTargetEmployeeSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                EmployeeId = employeeId
            };

            CommandResult commandResult = _targetEmployeeSettingService.DeleteTargetEmployeeSetting(criteria);
            return commandResult;
        }

        public IList<TargetDepartmentModel> GetTargetDepartmentSetting()
        {
            var criteria = new GetTargetDepartmentSettingCriteria();
            IList<TargetDepartmentModel> targetDepartments = _targetEmployeeSettingService.GetTargetDepartmentSetting(criteria);
            return targetDepartments;
        }

        [HttpPut]
        public CommandResult AssignTargetTemplateToDepartment(RawTargetSettingAssignModel model)
        {
            var criteria = new AssignTargetDepartmentSettingCriteria
            {
                TargetTemplateId = model.TargetTemplateId,
                Departments = model.ConvertToXml(),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            CommandResult commandResult = _targetEmployeeSettingService.AssignTargetTemplateToDepartment(criteria);
            return commandResult;
        }

        [HttpDelete]
        public CommandResult DeleteTargetDepartmentSetting(
            int targetTemplateId,
            int departmentId,
            bool isGroup)
        {
            var criteria = new DeleteTargetDepartmentSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                DepartmentId = departmentId,
                IsGroup = isGroup
            };

            CommandResult commandResult = _targetEmployeeSettingService.DeleteTargetDepartmentSetting(criteria);
            return commandResult;
        }
    }
}

namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class TargetEmployeeSettingController : BaseController
    {
        private readonly ITargetEmployeeSettingService targetEmployeeSettingService;

        public TargetEmployeeSettingController() : this(null)
        {
        }

        public TargetEmployeeSettingController(ITargetEmployeeSettingService targetEmployeeSettingService)
        {
            this.targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetEmployeeSetting> GetTargetEmployeeSetting(
            int? groupId = null,
            int? employeeId = null,
            int? targetTypeId = null,
            bool showAll = false)
        {
            var criteria = new GetAllTargetEmployeeSettingCriteria
            {
                GroupId = groupId,
                EmployeeId = employeeId,
                TargetTypeId = targetTypeId,
                ShowAll = showAll
            };

            IList<TargetEmployeeSetting> targetEmployeeSettings = targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);
            return targetEmployeeSettings;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = targetEmployeeSettingService.AssignTargetTemplateToEmployee(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult DeleteTargetEmployeeSetting(int targetTemplateId, int employeeId)
        {
            var criteria = new DeleteTargetEmployeeSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                EmployeeId = employeeId,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = targetEmployeeSettingService.DeleteTargetEmployeeSetting(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(
            int? departmentId = null,
            bool? isGroup = null,
            int? targetTypeId = null,
            bool showAll = false)
        {
            var criteria = new GetTargetDepartmentSettingCriteria
            {
                DepartmentId = departmentId,
                IsGroup = isGroup,
                TargetTypeId = targetTypeId,
                ShowAll = showAll
            };
            IList<TargetDepartmentSetting> targetDepartments = targetEmployeeSettingService.GetTargetDepartmentSetting(criteria);
            return targetDepartments;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult AssignTargetTemplateToDepartment(TargetSettingAssignModel model)
        {
            var commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new AssignTargetDepartmentSettingCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = SessionService.EmployeeId
                };

                commandResult = targetEmployeeSettingService.AssignTargetTemplateToDepartment(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult DeleteTargetDepartmentSetting(
            int targetTemplateId,
            int departmentId,
            bool isGroup)
        {
            var criteria = new DeleteTargetDepartmentSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                DepartmentId = departmentId,
                IsGroup = isGroup,
                ModifiedBy = SessionService.EmployeeId
            };

            CommandResult commandResult = targetEmployeeSettingService.DeleteTargetDepartmentSetting(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetEmployeeSetting()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.TargetEmployeeSetting);
            return permission;
        }
    }
}
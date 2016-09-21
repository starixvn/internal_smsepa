namespace SMS.EPA.Site.Controllers.API
{
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class TargetEmployeeSettingController : BaseController
    {
        private readonly ITargetEmployeeSettingService targetEmployeeSettingService;

        public TargetEmployeeSettingController()
            : this(null)
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
            bool? showAll = null)
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
                criteria.ModifiedBy = this.SessionService.EmployeeId;
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
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = targetEmployeeSettingService.DeleteTargetEmployeeSetting(criteria);
            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(
            int? departmentId = null,
            bool? isGroup = null,
            int? targetTypeId = null,
            bool? showAll = null)
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
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new AssignTargetDepartmentSettingCriteria
                {
                    TargetTemplateId = model.TargetTemplateId,
                    Departments = model.ConvertToXml(),
                    ModifiedBy = this.SessionService.EmployeeId
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
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = targetEmployeeSettingService.DeleteTargetDepartmentSetting(criteria);
            return commandResult;
        }

        public int GetUserPermissionOnTargetEmployeeSetting()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.TargetEmployeeSetting);
            return permission;
        }
    }
}
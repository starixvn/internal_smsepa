namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class TargetEmployeeSettingController : BaseController
    {
        private readonly ITargetEmployeeSettingService targetEmployeeSettingService;
        private readonly IBaseService baseService;

        public TargetEmployeeSettingController() : this(null, null)
        {
        }

        public TargetEmployeeSettingController(
            ITargetEmployeeSettingService targetEmployeeSettingService,
            IBaseService baseService)
        {
            this.targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
            this.baseService = baseService ?? new BaseService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetEmployeeSetting> GetTargetEmployeeSetting(
            string teamIds = null,
            int? employeeId = null,
            int? targetTypeId = null,
            bool showAll = false)
        {
            var criteria = new GetAllTargetEmployeeSettingCriteria
            {
                TeamIds = teamIds,
                EmployeeId = employeeId,
                TargetTypeId = targetTypeId,
                ShowAll = showAll
            };

            return targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Host)]
        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

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
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.TargetEmployeeSetting, Policy = Policy.Read)]
        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(
            string departmentIds = null,
            int? targetTypeId = null,
            bool showAll = false)
        {
            var criteria = new GetTargetDepartmentSettingCriteria
            {
                DepartmentIds = departmentIds,
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
                    ModifiedBy = CurrentEmployeeId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
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
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public byte GetUserPermissionOnTargetEmployeeSetting()
        {
            return GetPermissionOnFunction(EPAFunctionKey.TargetEmployeeSetting);
        }
    }
}
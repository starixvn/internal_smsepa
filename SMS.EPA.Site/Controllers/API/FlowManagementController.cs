namespace SMS.EPA.Site.Controllers.API
{
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Linq;

    public class FlowManagementController : BaseController
    {
        private readonly IFlowSettingService flowSettingService;

        public FlowManagementController()
            : this(null)
        {
        }

        public FlowManagementController(IFlowSettingService flowSettingService)
        {
            this.flowSettingService = flowSettingService ?? new FlowSettingService();
        }

        #region Department Flow Setting

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return flowSettingService.GetDepartmentFlow(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetDepartmentFlowById(
            int departmentId,
            int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByIdCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0
            };

            return flowSettingService.GetDepartmentFlowById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreateDepartmentFlow(InsertFlowModel model)
        {
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new CreateDepartmentFlowCriteria()
                {
                    DepartmentId = Math.Abs(model.DepartmentId),
                    FlowTemplateId = model.FlowTemplateId,
                    IsGroup = model.DepartmentId <= 0,
                    ModifiedBy = this.SessionService.EmployeeId,
                    FlowDetails = model.FlowDetails
                };

                commandResult = flowSettingService.CreateDepartmentFlow(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteDepartmentFlow(
            int departmentId,
            int flowTemplateId)
        {
            var criteria = new DeleteDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0,
                ModifiedBy = this.SessionService.EmployeeId
            };

            return flowSettingService.DeleteDepartmentFlow(criteria);
        }

        [HttpGet]
        public IList<Department> GetDepartmentHasSetting(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };

            IList<Department> departments = flowSettingService.GetDepartmentHasSetting(criteria);
            return departments;
        }

        #endregion Department Flow Setting

        #region Personal Flow Setting

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<PersonalFlowModel> GetPersonalFlow(int flowTemplateId)
        {
            var criteria = new GetPersonalFlowCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return flowSettingService.GetPersonalFlow(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetPersonalFlowById(
            int employeeId,
            int flowTemplateId)
        {
            var criteria = new GetPersonalFlowByIdCriteria()
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId
            };

            return flowSettingService.GetPersonalFlowById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreatePersonalFlow(InsertFlowModel model)
        {
            CommandResult commandResult = new CommandResult();

            if (model != null)
            {
                var criteria = new CreatePersonalFlowCriteria()
                {
                    EmployeeId = model.Selfs.Employees[0].EmployeeId,
                    FlowTemplateId = model.FlowTemplateId,
                    FlowDetails = model.FlowDetails,
                    ModifiedBy = this.SessionService.EmployeeId
                };

                commandResult = flowSettingService.CreatePersonalFlow(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeletePersonalFlow(
            int employeeId,
            int flowTemplateId)
        {
            var criteria = new DeletePersonalFlowCriteria()
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId,
                ModifiedBy = this.SessionService.EmployeeId
            };

            return flowSettingService.DeletePersonalFlow(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public EmployeeFlowSettingModel GetEmployeeFlowSetting(
            int flowTemplateId,
            int? departmentId = null,
            int? groupId = null)
        {
            IEnumerable<Employee> employees = new List<Employee>();
            var result = new EmployeeFlowSettingModel();

            var criteria = new GetEmployeeFlowSettingCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId,
                GroupId = groupId
            };

            employees = flowSettingService.GetEmployeeFlowSetting(criteria);

            if (employees != null)
            {
                result.DefaultFlowSettings = employees.Where(x => x.IsExistsSetting == false);
                result.PersonalFlowSettings = employees.Where(x => x.IsExistsSetting == true);
            }

            return result;
        }

        #endregion Personal Flow Setting

        public int GetUserPermissionOnFlowManagement()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.FlowManagement);
            return permission;
        }
    }
}
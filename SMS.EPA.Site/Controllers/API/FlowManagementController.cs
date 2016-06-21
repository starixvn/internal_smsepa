namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.FlowManagement;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;
    using SMS.EPA.Web.Services;

    public class FlowManagementController : BaseController
    {
        private readonly IFlowManagementService _flowManagementService;
        private readonly IWebCommonService _webCommonService;

        public FlowManagementController()
            : this(null, null)
        {
        }

        public FlowManagementController(
            IFlowManagementService flowManagementService,
            IWebCommonService webCommonService)
        {
            _flowManagementService = flowManagementService ?? new FlowManagementService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        #region Flow Template

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            var criteria = new GetFlowTemplateCriteria()
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return _flowManagementService.GetFlowTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public FlowTemplateModel GetFlowTemplateById(int flowTemplateId)
        {
            var criteria = new GetFlowTemplateByIdCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetFlowTemplateById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _flowManagementService.CreateFlowTemplate(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _flowManagementService.UpdateFlowTemplate(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _flowManagementService.UpdateStatusFlowTemplate(criteria);
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteFlowTemplate(int flowTemplateId)
        {
            var criteria = new DeleteFlowTemplateCriteria()
            {
                FlowTemplateId = flowTemplateId,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _flowManagementService.DeleteFlowTemplate(criteria);
        }

        #endregion Flow Template

        #region Department Flow Setting

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return _flowManagementService.GetDepartmentFlow(criteria);
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

            return _flowManagementService.GetDepartmentFlowById(criteria);
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
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
                    FlowDetails = model.FlowDetails
                };

                commandResult = _flowManagementService.CreateDepartmentFlow(criteria);
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
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _flowManagementService.DeleteDepartmentFlow(criteria);
        }

        [HttpGet]
        public IList<DepartmentModel> GetDepartmentHasSetting(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };

            IList<DepartmentModel> departments = _flowManagementService.GetDepartmentHasSetting(criteria);
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

            return _flowManagementService.GetPersonalFlow(criteria);
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

            return _flowManagementService.GetPersonalFlowById(criteria);
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
                    ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
                };

                commandResult = _flowManagementService.CreatePersonalFlow(criteria);
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
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _flowManagementService.DeletePersonalFlow(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public EmployeeFlowSettingModel GetEmployeeFlowSetting(
            int flowTemplateId,
            int? departmentId = null,
            int? groupId = null)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();
            var result = new EmployeeFlowSettingModel();

            var criteria = new GetEmployeeFlowSettingCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId,
                GroupId = groupId
            };

            employees = _flowManagementService.GetEmployeeFlowSetting(criteria);

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
            int permission = _webCommonService.GetPermission(EPAFunctionKey.FlowManagement);
            return permission;
        }
    }
}
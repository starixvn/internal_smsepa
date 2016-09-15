namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DepartmentFlowController : BaseController
    {
        private readonly IDepartmentFlowService departmentFlowService;

        public DepartmentFlowController()
            : this(null)
        {
        }

        public DepartmentFlowController(IDepartmentFlowService departmentFlowService)
        {
            this.departmentFlowService = departmentFlowService ?? new DepartmentFlowService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<DepartmentFlowModel> GetDepartmentFlow(int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return departmentFlowService.GetDepartmentFlow(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetDepartmentFlowById(int departmentId, int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByIdCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0
            };

            return departmentFlowService.GetDepartmentFlowById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.DepartmentId = Math.Abs(criteria.DepartmentId);
                criteria.ModifiedBy = this.SessionService.EmployeeId;

                commandResult = departmentFlowService.CreateDepartmentFlow(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteDepartmentFlow(int departmentId, int flowTemplateId)
        {
            var criteria = new DeleteDepartmentFlowCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0,
                ModifiedBy = this.SessionService.EmployeeId
            };

            return departmentFlowService.DeleteDepartmentFlow(criteria);
        }

        [HttpGet]
        public IList<Department> GetDepartmentHasSetting(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };

            IList<Department> departments = departmentFlowService.GetDepartmentHasSetting(criteria);
            return departments;
        }
    }
}
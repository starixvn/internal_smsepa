namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;

    public class DepartmentFlowController : BaseController
    {
        private readonly IDepartmentFlowService departmentFlowService;
        private readonly ICommonService commonService;

        public DepartmentFlowController() : this(null, null)
        {
        }

        public DepartmentFlowController(IDepartmentFlowService departmentFlowService, ICommonService commonService)
        {
            this.departmentFlowService = departmentFlowService ?? new DepartmentFlowService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<DepartmentFlowModel> GetDepartmentFlowByFlowTemplateId(int flowTemplateId)
        {
            return departmentFlowService.GetDepartmentFlowByFlowTemplateId(flowTemplateId);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetDepartmentFlowById(int departmentId, int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByIdCriteria
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
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.DepartmentId = Math.Abs(criteria.DepartmentId);
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteDepartmentFlow(int departmentId, int flowTemplateId)
        {
            var criteria = new DeleteDepartmentFlowCriteria
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0,
                ModifiedBy = SessionService.EmployeeId
            };

            return commonService.ExecuteNonQuery(criteria);
        }

        [HttpGet]
        public IList<Department> GetDepartmentHasSetting(int flowTemplateId)
        {
            return departmentFlowService.GetDepartmentHasSettingByFlowTemplateId(flowTemplateId);
        }
    }
}
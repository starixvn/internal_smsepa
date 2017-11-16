namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;

    public class DepartmentFlowController : BaseController
    {
        private readonly IDepartmentFlowService departmentFlowService;
        private readonly IBaseService baseService;

        public DepartmentFlowController() : this(null, null)
        {
        }

        public DepartmentFlowController(IDepartmentFlowService departmentFlowService, IBaseService baseService)
        {
            this.departmentFlowService = departmentFlowService ?? new DepartmentFlowService();
            this.baseService = baseService ?? new BaseService();
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

                commandResult = baseService.ExecuteNonQuery(criteria);
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

            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpGet]
        public IEnumerable<Department> GetDepartmentHasSetting(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };

            return baseService.GetData<Department>(criteria);
        }
    }
}
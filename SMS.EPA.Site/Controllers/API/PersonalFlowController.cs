namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class PersonalFlowController : BaseController
    {
        private readonly IPersonalFlowService personalFlowService;
        private readonly ICommonService commonService;

        public PersonalFlowController() : this(null, null)
        {
        }

        public PersonalFlowController(IPersonalFlowService personalFlowService, ICommonService commonService)
        {
            this.personalFlowService = personalFlowService ?? new PersonalFlowService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(int flowTemplateId)
        {
            return personalFlowService.GetPersonalFlowByFlowTemplateId(flowTemplateId);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetPersonalFlowById(int employeeId, int flowTemplateId)
        {
            var criteria = new GetPersonalFlowByIdCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId
            };

            return personalFlowService.GetPersonalFlowById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = commonService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeletePersonalFlow(int employeeId, int flowTemplateId)
        {
            var criteria = new DeletePersonalFlowCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId,
                ModifiedBy = SessionService.EmployeeId
            };

            return commonService.ExecuteNonQuery(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public EmployeeFlowSettingModel GetEmployeeFlowSetting(
            int flowTemplateId,
            int? departmentId = null,
            int? groupId = null,
            bool? isMonthlyPeriod = null)
        {
            var criteria = new GetEmployeeFlowSettingCriteria
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId,
                GroupId = groupId,
                IsMonthlyPeriod = isMonthlyPeriod
            };

            EmployeeFlowSettingModel result = personalFlowService.GetEmployeeFlowSetting(criteria);
            return result;
        }
    }
}
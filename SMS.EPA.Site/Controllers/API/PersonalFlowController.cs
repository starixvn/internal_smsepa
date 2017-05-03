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

    public class PersonalFlowController : BaseController
    {
        private readonly IPersonalFlowService _personalFlowService;

        public PersonalFlowController()
            : this(null)
        {
        }

        public PersonalFlowController(IPersonalFlowService personalFlowService)
        {
            _personalFlowService = personalFlowService ?? new PersonalFlowService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<PersonalFlowModel> GetPersonalFlowByFlowTemplateId(int flowTemplateId)
        {
            return _personalFlowService.GetPersonalFlowByFlowTemplateId(flowTemplateId);
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

            return _personalFlowService.GetPersonalFlowById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;

                commandResult = _personalFlowService.CreatePersonalFlow(criteria);
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

            return _personalFlowService.DeletePersonalFlow(criteria);
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

            EmployeeFlowSettingModel result = _personalFlowService.GetEmployeeFlowSetting(criteria);
            return result;
        }
    }
}
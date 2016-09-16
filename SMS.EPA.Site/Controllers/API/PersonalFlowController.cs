namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class PersonalFlowController : BaseController
    {
        private readonly IPersonalFlowService personalFlowService;

        public PersonalFlowController()
            : this(null)
        {
        }

        public PersonalFlowController(IPersonalFlowService personalFlowService)
        {
            this.personalFlowService = personalFlowService ?? new PersonalFlowService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<PersonalFlowModel> GetPersonalFlow(int flowTemplateId)
        {
            return personalFlowService.GetPersonalFlowByFlowTemplateId(flowTemplateId);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetPersonalFlowById(int employeeId, int flowTemplateId)
        {
            var criteria = new GetPersonalFlowByIdCriteria()
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
            CommandResult commandResult = new CommandResult();

            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;

                commandResult = personalFlowService.CreatePersonalFlow(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeletePersonalFlow(int employeeId, int flowTemplateId)
        {
            var criteria = new DeletePersonalFlowCriteria()
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId,
                ModifiedBy = this.SessionService.EmployeeId
            };

            return personalFlowService.DeletePersonalFlow(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public EmployeeFlowSettingModel GetEmployeeFlowSetting(int flowTemplateId, int? departmentId = null, int? groupId = null)
        {
            var criteria = new GetEmployeeFlowSettingCriteria()
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId,
                GroupId = groupId
            };

            EmployeeFlowSettingModel result = personalFlowService.GetEmployeeFlowSetting(criteria);
            return result;
        }
    }
}
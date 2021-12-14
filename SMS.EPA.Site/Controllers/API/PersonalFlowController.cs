namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services;

    public class PersonalFlowController : BaseController
    {
        private readonly IPersonalFlowService personalFlowService;
        private readonly IBaseService baseService;

        public PersonalFlowController() : this(null, null)
        {
        }

        public PersonalFlowController(IPersonalFlowService personalFlowService, IBaseService baseService)
        {
            this.personalFlowService = personalFlowService ?? new PersonalFlowService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<PersonalFlowModel> GetByFlowTemplateId(int flowTemplateId)
        {
            return personalFlowService.GetByFlowTemplateId(flowTemplateId);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetById(int employeeId, int flowTemplateId)
        {
            var criteria = new GetPersonalFlowByIdCriteria
            {
                EmployeeId = employeeId,
                FlowTemplateId = flowTemplateId
            };
            return personalFlowService.GetById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreatePersonalFlow(CreatePersonalFlowCriteria criteria)
        {
            var commandResult = new CommandResult();
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
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
            return baseService.ExecuteNonQuery(criteria);
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

            var result = new EmployeeFlowSettingModel();
            var data = baseService.GetData<EvaluatorInfoViewModel>(criteria).OrderBy(i => i.NickName);
            result.DefaultFlowSettings = data.Where(x => !x.IsExistsSetting);
            result.PersonalFlowSettings = data.Where(x => x.IsExistsSetting);

            return result;
        }
    }
}
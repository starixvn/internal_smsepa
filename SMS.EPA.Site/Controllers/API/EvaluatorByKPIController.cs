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
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EvaluatorByKPIController : BaseController
    {
        private readonly IEvaluatorByKPIService evaluatorByKPIService;

        public EvaluatorByKPIController()
            : this(null)
        {
        }

        public EvaluatorByKPIController(IEvaluatorByKPIService evaluatorByKPIService)
        {
            this.evaluatorByKPIService = evaluatorByKPIService ?? new EvaluatorByKPIService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator()
        {
            var criteria = new GetPeriodByEvaluatorCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId
            };

            return evaluatorByKPIService.GetPeriodByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetTargetTemplateByEvaluator(int periodId)
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                PeriodId = periodId
            };

            return evaluatorByKPIService.GetTargetTemplateByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodToView()
        {
            var criteria = new GetPeriodToViewCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId
            };

            return evaluatorByKPIService.GetPeriodToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetTargetTemplateToView(int periodId)
        {
            var criteria = new GetTargetTemplateToViewCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                PeriodId = periodId
            };

            return evaluatorByKPIService.GetTargetTemplateToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetEvaluatorsByTargetTemplateCriteria()
            {
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return evaluatorByKPIService.GetEvaluatorsByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIToView(
            int periodId,
            int targetTemplateId,
            int evaluatorId)
        {
            var criteria = new GetKPIToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return evaluatorByKPIService.GetKPIToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int evaluatorId)
        {
            var criteria = new GetEmployeeEPAToViewCriteria()
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId
            };

            return evaluatorByKPIService.GetEmployeeEPAToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                TargetTemplateId = targetTemplateId,
                PeriodId = periodId
            };

            return evaluatorByKPIService.GetKPIByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEmployeeEPAByKPICriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return evaluatorByKPIService.GetEmployeeEpabyKPI(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Host)]
        public CommandResult UpdateKPIScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var criteria = new UpdateKPIScoreCriteria()
            {
                EvaluatorId = this.SessionService.EmployeeId,
                EpaInfos = epaInfos
            };

            return evaluatorByKPIService.UpdateKPIScore(criteria);
        }

        public int GetUserPermissionOnRatingByKPI()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.RatingByKPI);
            return permission;
        }
    }
}
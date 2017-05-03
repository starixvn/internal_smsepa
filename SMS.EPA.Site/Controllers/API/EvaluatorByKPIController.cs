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
    using System.Collections.Generic;
    using System.Web.Http;

    public class EvaluatorByKPIController : BaseController
    {
        private readonly IEvaluatorByKPIService _evaluatorByKPIService;

        public EvaluatorByKPIController()
            : this(null)
        {
        }

        public EvaluatorByKPIController(IEvaluatorByKPIService evaluatorByKPIService)
        {
            _evaluatorByKPIService = evaluatorByKPIService ?? new EvaluatorByKPIService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator()
        {
            var criteria = new GetPeriodByEvaluatorCriteria
            {
                EvaluatorId = SessionService.EmployeeId
            };

            return _evaluatorByKPIService.GetPeriodByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetTargetTemplateByEvaluator(int periodId)
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                PeriodId = periodId
            };

            return _evaluatorByKPIService.GetTargetTemplateByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<EPAPeriodModel> GetPeriodToView()
        {
            var criteria = new GetPeriodToViewCriteria
            {
                EvaluatorId = SessionService.EmployeeId
            };

            return _evaluatorByKPIService.GetPeriodToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplate> GetTargetTemplateToView(int periodId)
        {
            var criteria = new GetTargetTemplateToViewCriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                PeriodId = periodId
            };

            return _evaluatorByKPIService.GetTargetTemplateToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetEvaluatorsByTargetTemplateCriteria
            {
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return _evaluatorByKPIService.GetEvaluatorsByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIToView(
            int periodId,
            int targetTemplateId,
            int evaluatorId)
        {
            var criteria = new GetKPIToViewCriteria
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId
            };

            return _evaluatorByKPIService.GetKPIToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPAByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(
            int periodId,
            int targetTemplateId,
            int kpiId,
            int evaluatorId)
        {
            var criteria = new GetEmployeeEPAToViewCriteria
            {
                EvaluatorId = evaluatorId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId
            };

            return _evaluatorByKPIService.GetEmployeeEPAToView(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(
            int periodId,
            int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                TargetTemplateId = targetTemplateId,
                PeriodId = periodId
            };

            return _evaluatorByKPIService.GetKPIByTargetTemplate(criteria);
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
            var criteria = new GetEmployeeEPAByKPICriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                PeriodId = periodId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _evaluatorByKPIService.GetEmployeeEpabyKPI(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Host)]
        public CommandResult UpdateKPIScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            var criteria = new UpdateKPIScoreCriteria
            {
                EvaluatorId = SessionService.EmployeeId,
                EpaInfos = epaInfos
            };

            return _evaluatorByKPIService.UpdateKPIScore(criteria);
        }

        public int GetUserPermissionOnRatingByKPI()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.RatingByKPI);
            return permission;
        }
    }
}
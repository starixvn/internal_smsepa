// -----------------------------------------------------------------------
// <copyright file="EmployeeEPAController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.10">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _employeeEPAService;

        public EmployeeEPAController()
            : this(null)
        {
        }

        public EmployeeEPAController(IEmployeeEPAService employeeEPAService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        #region Self

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEmployee()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Read)]
        public EPAModel GetEPAInfoByEmployee(int epaId)
        {
            var criteria = new GetEPAInfoByEmployeeCriteria()
            {
                EPAId = epaId,
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAInfoByEmployee(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Self, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEmployee(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = (int)ProcessType.Self,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.CurrentProcess.Comment,
                EPAProcess = model.CurrentProcess,
                KPIGroups = model.KPIGroups,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return UpdateEPAScoreByEmployeeOrEvaluator(criteria);
        }

        #endregion Self

        #region Evaluator

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByEvaluator(int? groupId = null)
        {
            var criteria = new GetMemberCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = (int)ProcessType.Evaluator,
                GroupId = groupId
            };

            return GetMemberEvaluatorOrReivewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByEvaluator(
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Read)]
        public EPAModel GetEPAInfoByEvaluator(int epaId)
        {
            var criteria = new GetEPAInfoByEvaluatorCriteria()
            {
                EPAId = epaId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAInfoByEvaluator(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Evaluator, Policy = Policy.Host)]
        public CommandResult UpdateEPAScoreByEvaluator(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = (int)ProcessType.Evaluator,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.ProcessByEvaluators.FirstOrDefault().Comment,
                EPAProcess = model.ProcessByEvaluators.FirstOrDefault(),
                KPIGroups = model.KPIGroups,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return UpdateEPAScoreByEmployeeOrEvaluator(criteria);
        }

        #endregion Evaluator

        #region Reviewer

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByReviewer(int? groupId = null)
        {
            var criteria = new GetMemberCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = (int)ProcessType.Reviewer,
                GroupId = groupId
            };

            return GetMemberEvaluatorOrReivewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByReviewer(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Read)]
        public EPAModel GetEPAInfoByReviewer(int epaId)
        {
            var criteria = new GetEPAInfoByReviewerCriteria()
            {
                EPAId = epaId,
                ReviewerId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAInfoByReviewer(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Reviewer, Policy = Policy.Host)]
        public CommandResult AdjustScoreByReviewer(IEnumerable<EPAModel> memberEPAs)
        {
            var criteria = new AdjustScoreByReviewerOrManagementCriteria()
            {
                ProcessId = (int)ProcessType.Reviewer,
                MemberEPAs = memberEPAs,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
        }

        #endregion Reviewer

        #region Management

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<Employee> GetMemberByManagement(int? groupId = null)
        {
            var criteria = new GetMemberCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = (int)ProcessType.Management,
                GroupId = groupId
            };

            return GetMemberEvaluatorOrReivewerOrManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEPAByManagementCriteria()
            {
                ManagementId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEPAByManagement(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Read)]
        public EPAModel GetEPAInfoByManagement(int epaId)
        {
            var criteria = new GetEPAInfoByManagementCriteria()
            {
                EPAId = epaId,
                ManagementId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAInfoByManagement(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Management, Policy = Policy.Host)]
        public CommandResult AdjustScoreByManagement(IEnumerable<EPAModel> memberEPAs)
        {
            var criteria = new AdjustScoreByReviewerOrManagementCriteria()
            {
                ProcessId = (int)ProcessType.Management,
                MemberEPAs = memberEPAs,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return AdjustScoreByReviewerOrManagement(criteria);
        }

        #endregion Management

        #region Rating By KPI

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator()
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetTargetTemplateByEvaluator(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria()
            {
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetKPIByTargetTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Read)]
        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(
            int targetTemplateId,
            int kpiId,
            int? groupId = null,
            string employeeIds = null)
        {
            var criteria = new GetEmployeeEPAByKPICriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTemplateId = targetTemplateId,
                KPIId = kpiId,
                GroupId = groupId,
                EmployeeIds = employeeIds
            };

            return _employeeEPAService.GetEmployeeEPAByKPI(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RatingByKPI, Policy = Policy.Host)]
        public CommandResult UpdateKPIScore(IEnumerable<RatingByKPIModel> models)
        {
            var criteria = new UpdateKPIScoreCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                RatingByKPIs = models
            };

            return _employeeEPAService.UpdateKPIScore(criteria);
        }

        #endregion Rating By KPI

        [HttpGet]
        public EPAModel GetEPAInfoById(int epaId)
        {
            var criteria = new GetEPAInfoByIdCriteria()
            {
                EPAId = epaId
            };

            return _employeeEPAService.GetEPAInfoById(criteria);
        }

        private CommandResult UpdateEPAScoreByEmployeeOrEvaluator(UpdateEPAScoreCriteria criteria)
        {
            return _employeeEPAService.UpdateEPAScore(criteria);
        }

        private IEnumerable<Employee> GetMemberEvaluatorOrReivewerOrManagement(GetMemberCriteria criteria)
        {
            return _employeeEPAService.GetMember(criteria);
        }

        private CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            return _employeeEPAService.AdjustScoreByReviewerOrManagement(criteria);
        }
    }
}
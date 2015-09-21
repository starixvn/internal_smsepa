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

        [HttpGet]
        public IEnumerable<Employee> GetMember(
            int processId,
            int? groupId = null)
        {
            var criteria = new GetMemberCriteria()
            {
                ActorUserId = CurrentUser.EmployeeInfo.EmployeeId,
                ProcessId = processId,
                GroupId = groupId
            };

            return _employeeEPAService.GetMember(criteria);
        }

        #region Self

        [HttpGet]
        public IEnumerable<EPAModel> GetEPAOfCurrentMember()
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        public EPAModel GetEPAInfoByEmployee(int epaId)
        {
            var criteria = new GetEPAInfoByEmployeeCriteria()
            {
                EPAId = epaId,
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAInfoByEmployee(criteria);
        }

        #endregion Self

        #region Evaluator

        [HttpGet]
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
        public CommandResult UpdateEPAScoreByEvaluator(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = model.ProcessByEvaluators.FirstOrDefault().ProcessId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                EPAProcess = model.ProcessByEvaluators.FirstOrDefault(),
                KPIGroups = model.KPIGroups,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return _employeeEPAService.UpdateEPAScore(criteria);
        }

        #endregion Evaluator

        #region Reviewer

        [HttpGet]
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
        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator()
        {
            var criteria = new GetTargetTemplateByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetTargetTemplateByEvaluator(criteria);
        }

        [HttpGet]
        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(int targetTemplateId)
        {
            var criteria = new GetKPIByTargetTemplateCriteria()
            {
                TargetTemplateId = targetTemplateId
            };

            return _employeeEPAService.GetKPIByTargetTemplate(criteria);
        }

        [HttpGet]
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

        [HttpPut]
        public CommandResult UpdateEPAScore(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = model.ProcessBySelf.ProcessId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                EPAProcess = model.ProcessBySelf,
                KPIGroups = model.KPIGroups,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return _employeeEPAService.UpdateEPAScore(criteria);
        }

        [HttpPut]
        public CommandResult UpdateScoreByReviewerOrManagement(UpdateScoreByReviewerOrManagementCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                criteria.Score = _employeeEPAService.Encode(criteria.Score);
            }

            return _employeeEPAService.UpdateScoreByReviewerOrManagement(criteria);
        }

        private CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            return _employeeEPAService.AdjustScoreByReviewerOrManagement(criteria);
        }
    }
}
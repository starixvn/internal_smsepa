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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAController : BaseController
    {
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly UtilitiesController _utilitiesController;

        public EmployeeEPAController()
            : this(null, null)
        {
        }

        public EmployeeEPAController(
            IEmployeeEPAService employeeEPAService,
            UtilitiesController utilitiesController)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _utilitiesController = utilitiesController ?? new UtilitiesController();
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
        public IEnumerable<EmployeeEPAModel> GetEPAOfCurrentMember(
            int? targetTypeId = null,
            int? status = null)
        {
            var criteria = new GetEPAByEmployeeCriteria
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
                Status = status
            };

            return _employeeEPAService.GetEPAByEmployee(criteria);
        }

        [HttpGet]
        public EmployeeEPAInfoModel GetEPAInfoByEmployee(
            int epaId,
            int periodId,
            int employeeId)
        {
            EmployeeEPAInfoModel epaInfo = new EmployeeEPAInfoModel();

            var criteria = new GetEPAInfoCriteria()
            {
                EPAId = epaId,
                PeriodId = periodId,
                EmployeeId = employeeId
            };

            epaInfo = _employeeEPAService.GetEPAInfo(criteria);
            epaInfo.Employee = _utilitiesController.GetEmployeeById(employeeId);

            return epaInfo;
        }

        #endregion Self

        #region Evaluator

        [HttpGet]
        public IEnumerable<MemberEPAModel> GetEPAByEvaluator(
            DateTime fromDate,
            DateTime toDate,
            int? targetTypeId = null,
            int? groupId = null,
            string employeeIds = null,
            int? status = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                FromDate = fromDate,
                ToDate = toDate,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
                GroupId = groupId,
                EmployeeIds = employeeIds,
                Status = status
            };

            return _employeeEPAService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        public EmployeeEPAInfoModel GetEPAInfoByEvaluator(
            int epaId,
            int periodId,
            int employeeId,
            bool isViewMode,
            int? evaluatorId = null)
        {
            EmployeeEPAInfoModel memberEPAInfo = new EmployeeEPAInfoModel();

            var criteria = new GetEPAInfoByEvaluatorCriteria()
            {
                EPAId = epaId,
                PeriodId = periodId,
                EmployeeId = employeeId
            };

            if (isViewMode && evaluatorId > 0)
            {
                criteria.EvaluatorId = (int)evaluatorId;
            }
            else
            {
                criteria.EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId;
            }

            memberEPAInfo = _employeeEPAService.GetEPAInfoByEvaluator(criteria);
            memberEPAInfo.Employee = _utilitiesController.GetEmployeeById(employeeId);

            return memberEPAInfo;
        }

        #endregion Evaluator

        #region Reviewer

        [HttpGet]
        public IEnumerable<MemberEPAModel> GetEPAByReviewer(
            DateTime fromDate,
            DateTime toDate,
            int? targetTypeId = null,
            int? groupId = null,
            string employeeIds = null,
            int? status = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                FromDate = fromDate,
                ToDate = toDate,
                ReviewerId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
                GroupId = groupId,
                EmployeeIds = employeeIds,
                Status = status
            };

            return _employeeEPAService.GetEPAByReviewer(criteria);
        }

        #endregion Reviewer

        [HttpPut]
        public CommandResult UpdateEmployeeEPA(EmployeeEPAInfoModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = model.ProcessId,
                PeriodId = model.PeriodId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.CommentBySelf,
                Score = EncodeTotalScore(model),
                KPIDetails = ConvertKPIInfoToXml(model.KPIGroups, model.ProcessId),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return _employeeEPAService.UpdateEmployeeEPA(criteria);
        }

        [HttpPut]
        public CommandResult UpdateStatusEPA(UpdateStatusEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _employeeEPAService.UpdateStatusEPA(criteria);
        }

        private string EncodeTotalScore(EmployeeEPAInfoModel model)
        {
            string score = string.Empty;

            if (model.ProcessId == (int)ProcessType.Self)
            {
                score = _employeeEPAService.Encode(model.TotalScoreBySelf);
            }
            else if (model.ProcessId == (int)ProcessType.Evaluator)
            {
                score = _employeeEPAService.Encode(model.TotalScoreByEvaluator);
            }

            return score;
        }

        private string ConvertKPIInfoToXml(IList<KPIGroupViewModel> kpiGroups, int processId)
        {
            string beginRootTag = "<ROOT>";
            string endRootTag = "</ROOT>";
            string cipherScore = string.Empty;
            string employeeEPAScore = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' Comment='{4}'></KPI>";
            var epaXML = new StringBuilder(beginRootTag);

            foreach (KPIGroupViewModel kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpiGroup.KPIs)
                {
                    if (processId == (int)ProcessType.Self)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.ScoreBySelf) ? _employeeEPAService.Encode(kpi.ScoreBySelf + "_" + kpi.KPIId) : null;
                        epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.RateBySelf, HttpUtility.HtmlEncode(kpi.CommentBySelf));
                    }
                    else if (processId == (int)ProcessType.Evaluator)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.ScoreByEvaluator) ? _employeeEPAService.Encode(kpi.ScoreByEvaluator + "_" + kpi.KPIId) : null;
                        epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.RateByEvaluator, HttpUtility.HtmlEncode(kpi.CommentByEvaluator));
                    }
                }
            }

            epaXML = epaXML.Append(endRootTag);

            return epaXML.ToString();
        }
    }
}
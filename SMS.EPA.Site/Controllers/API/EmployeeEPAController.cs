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
            int? targetTypeId = null,
            int? groupId = null,
            string employeeIds = null,
            int? status = null)
        {
            var criteria = new GetEPAByEvaluatorCriteria()
            {
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
                GroupId = groupId,
                EmployeeIds = employeeIds,
                Status = status
            };

            return _employeeEPAService.GetEPAByEvaluator(criteria);
        }

        [HttpGet]
        public EPAModel GetEPAInfoByEvaluator(
            int epaId,
            int employeeId,
            int? evaluatorId = null)
        {
            var criteria = new GetEPAInfoByEvaluatorCriteria()
            {
                EPAId = epaId,
                EmployeeId = employeeId
            };

            if (evaluatorId != null)
            {
                criteria.EvaluatorId = (int)evaluatorId;
            }
            else
            {
                criteria.EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _employeeEPAService.GetEPAInfoByEvaluator(criteria);
        }

        [HttpPut]
        public CommandResult UpdateEPAScoreByEvaluator(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = model.ProcessByEvaluators[0].ProcessId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.ProcessByEvaluators[0].Comment,
                Score = EncodeTotalScore(model, model.ProcessByEvaluators[0].ProcessId),
                KPIDetails = ConvertKPIInfoToXml(model.KPIGroups, model.ProcessByEvaluators[0].ProcessId),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return _employeeEPAService.UpdateEPAScore(criteria);
        }

        #endregion Evaluator

        #region Reviewer

        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByReviewer(
            int? targetTypeId = null,
            int? groupId = null,
            string employeeIds = null,
            int? status = null)
        {
            var criteria = new GetEPAByReviewerCriteria()
            {
                ReviewerId = CurrentUser.EmployeeInfo.EmployeeId,
                TargetTypeId = targetTypeId,
                GroupId = groupId,
                EmployeeIds = employeeIds,
                Status = status
            };

            return _employeeEPAService.GetEPAByReviewer(criteria);
        }

        [HttpGet]
        public EPAModel GetEPAInfoByReviewer(
            int epaId,
            int employeeId,
            int evaluatorId)
        {
            var criteria = new GetEPAInfoByReviewerCriteria()
            {
                EPAId = epaId,
                EmployeeId = employeeId,
                EvaluatorId = evaluatorId,
                ReviewerId = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeEPAService.GetEPAInfoByReviewer(criteria);
        }

        #endregion Reviewer

        #region Management

        [HttpGet]
        public IEnumerable<EPAModel> GetEPAByManagement(
            int? groupId = null,
            string employeeIds = null,
            int? status = null)
        {
            var criteria = new GetEPAByManagementCriteria()
            {
                ManagementId = CurrentUser.EmployeeInfo.EmployeeId,
                GroupId = groupId,
                EmployeeIds = employeeIds,
                Status = status
            };

            return _employeeEPAService.GetEPAByManagement(criteria);
        }

        [HttpGet]
        public EPAModel GetEPAInfoByManagement(int epaId)
        {
            var criteria = new GetEPAInfoByManagementCriteria()
            {
                EPAId = epaId
            };

            return _employeeEPAService.GetEPAInfoByManagement(criteria);
        }

        #endregion Management

        [HttpPut]
        public CommandResult UpdateEPAScore(EPAModel model)
        {
            var criteria = new UpdateEPAScoreCriteria()
            {
                EPAId = model.EPAId,
                ProcessId = model.ProcessBySelf.ProcessId,
                EmployeeId = model.Employee.EmployeeId,
                EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId,
                Comment = model.ProcessBySelf.Comment,
                Score = EncodeTotalScore(model, model.ProcessBySelf.ProcessId),
                KPIDetails = ConvertKPIInfoToXml(model.KPIGroups, model.ProcessBySelf.ProcessId),
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

        [HttpPut]
        public CommandResult UpdateStatusEPA(UpdateStatusEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EvaluatorId = CurrentUser.EmployeeInfo.EmployeeId;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            return _employeeEPAService.UpdateStatusEPA(criteria);
        }

        private string EncodeTotalScore(EPAModel model, int processId)
        {
            string score = string.Empty;

            if (processId == (int)ProcessType.Self)
            {
                score = _employeeEPAService.Encode(model.ProcessBySelf.TotalScore);
            }
            else if (processId == (int)ProcessType.Evaluator)
            {
                score = _employeeEPAService.Encode(model.ProcessByEvaluators[0].TotalScore);
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

            foreach (var kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpiGroup.KPIs)
                {
                    if (processId == (int)ProcessType.Self)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.EvaluateBySelf.Score) ? _employeeEPAService.Encode(kpi.EvaluateBySelf.Score + "_" + kpi.KPIId) : null;
                        epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.EvaluateBySelf.Rate, HttpUtility.HtmlEncode(kpi.EvaluateBySelf.Comment));
                    }
                    else if (processId == (int)ProcessType.Evaluator)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.EvaluateByEvaluators[0].Score) ? _employeeEPAService.Encode(kpi.EvaluateByEvaluators[0].Score + "_" + kpi.KPIId) : null;
                        epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.EvaluateByEvaluators[0].Rate, HttpUtility.HtmlEncode(kpi.EvaluateByEvaluators[0].Comment));
                    }
                }
            }

            epaXML = epaXML.Append(endRootTag);

            return epaXML.ToString();
        }
    }
}
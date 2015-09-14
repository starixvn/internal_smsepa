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
                KPIDetails = ConvertKPIScoreToXml(models)
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

        private string ConvertKPIInfoToXml(IList<EPAKPIGroupModel> kpiGroups, int processId)
        {
            string cipherScore = string.Empty;
            string employeeEPAScore = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' Comment='{4}'></KPI>";
            var epaXML = new StringBuilder("<ROOT>");

            foreach (var kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpiGroup.KPIs)
                {
                    if (processId == (int)ProcessType.Self)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.EvaluateBySelf.Score) ? _employeeEPAService.Encode(kpi.EvaluateBySelf.Score + "_" + kpi.KPIId) : string.Empty;
                        epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.EvaluateBySelf.Rate, HttpUtility.HtmlEncode(kpi.EvaluateBySelf.Comment));
                    }
                    else if (processId == (int)ProcessType.Evaluator)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.EvaluateByEvaluators[0].Score) ? _employeeEPAService.Encode(kpi.EvaluateByEvaluators[0].Score + "_" + kpi.KPIId) : string.Empty;
                        epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.EvaluateByEvaluators[0].Rate, HttpUtility.HtmlEncode(kpi.EvaluateByEvaluators[0].Comment));
                    }
                }
            }

            epaXML = epaXML.Append("</ROOT>");

            return epaXML.ToString();
        }

        private string ConvertKPIScoreToXml(IEnumerable<RatingByKPIModel> models)
        {
            string cipherScore = string.Empty;
            string epa = "<EPA EPAId='{0}' EmployeeId='{1}' KPIGroupId='{2}' TotalScore='{3}'>{4}</EPA>";
            string epaDetail = "<EPADetail KPIId ='{0}' Rate='{1}' Score='{2}' Comment='{3}' />";

            var result = new StringBuilder("<ROOT>");

            foreach (var model in models)
            {
                var detail = new StringBuilder();

                foreach (var kpi in model.KPIGroup.KPIs)
                {
                    if (kpi.Score != kpi.AdjustScore && kpi.AdjustRate > 0)
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.AdjustScore) ? _employeeEPAService.Encode(kpi.AdjustScore + "_" + kpi.KPIId) : string.Empty;
                        detail.AppendFormat(epaDetail, kpi.KPIId, kpi.AdjustRate, cipherScore, HttpUtility.HtmlEncode(kpi.Comment));
                    }
                    else
                    {
                        cipherScore = !string.IsNullOrEmpty(kpi.Score) ? _employeeEPAService.Encode(kpi.Score + "_" + kpi.KPIId) : string.Empty;
                        detail.AppendFormat(epaDetail, kpi.KPIId, kpi.Rate, cipherScore, HttpUtility.HtmlEncode(kpi.Comment));
                    }
                }

                cipherScore = !string.IsNullOrEmpty(model.TotalScore) ? _employeeEPAService.Encode(model.TotalScore) : string.Empty;
                result.AppendFormat(epa, model.EPAId, model.Employee.EmployeeId, model.KPIGroup.KPIGroupId, cipherScore, detail);
            }

            result.Append("</ROOT>");

            return result.ToString();
        }
    }
}
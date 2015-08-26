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
        public IEnumerable<EPAModel> GetEPAOfCurrentMember(
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
        public EPAModel GetEPAInfoByEmployee(
            int epaId,
            int employeeId)
        {
            var criteria = new GetEPAInfoCriteria()
            {
                EPAId = epaId,
                EmployeeId = employeeId
            };

            return _employeeEPAService.GetEPAInfoByEmployee(criteria);
        }

        #endregion Self

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
                Score = EncodeTotalScore(model),
                KPIDetails = ConvertKPIInfoToXml(model.EPAInfoBySelf),
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId,
            };

            return _employeeEPAService.UpdateEPAScore(criteria);
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

        private string EncodeTotalScore(EPAModel model)
        {
            string score = string.Empty;

            if (model.ProcessBySelf.ProcessId == (int)ProcessType.Self)
            {
                score = _employeeEPAService.Encode(model.ProcessBySelf.TotalScore);
            }

            return score;
        }

        private string ConvertKPIInfoToXml(IList<EPAInfoModel> epaInfos)
        {
            string beginRootTag = "<ROOT>";
            string endRootTag = "</ROOT>";
            string cipherScore = string.Empty;
            string employeeEPAScore = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' Comment='{4}'></KPI>";
            var epaXML = new StringBuilder(beginRootTag);

            foreach (var epaInfo in epaInfos)
            {
                foreach (var kpi in epaInfo.KPIGroup.KPIs)
                {
                    cipherScore = !string.IsNullOrEmpty(kpi.Score) ? _employeeEPAService.Encode(kpi.Score + "_" + kpi.KPIId) : null;
                    epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, epaInfo.KPIGroup.KPIGroupId, cipherScore, kpi.Rate, HttpUtility.HtmlEncode(kpi.Comment));
                }
            }

            epaXML = epaXML.Append(endRootTag);

            return epaXML.ToString();
        }
    }
}
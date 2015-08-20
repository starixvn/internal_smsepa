// -----------------------------------------------------------------------
// <copyright file="MemberEPAService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;
        private readonly IEncodeService _encodeService;

        public EmployeeEPAService()
            : this(null, null)
        {
        }

        public EmployeeEPAService(
            IEmployeeEPARepository employeeEPARepository,
            IEncodeService encodeService)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            _encodeService = encodeService ?? new EncodeService();
        }

        public IEnumerable<EmployeeEPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EmployeeEPAModel> employeeEPAs = new List<EmployeeEPAModel>();

            if (criteria.IsValid())
            {
                employeeEPAs = _employeeEPARepository.GetEPAByEmployee(criteria);

                foreach (var employeeEPA in employeeEPAs)
                {
                    employeeEPA.Status.Name = Enumerations.GetDescription((Status)employeeEPA.Status.StatusId);
                    employeeEPA.Score = !string.IsNullOrEmpty(employeeEPA.Score) ? Decode(employeeEPA.Score) : string.Empty;
                }
            }

            return employeeEPAs;
        }

        public EmployeeEPAInfoModel GetEPAInfo(GetEPAInfoCriteria criteria)
        {
            EmployeeEPAInfoModel result = new EmployeeEPAInfoModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfo(criteria);

                if (result != null)
                {
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.KPIGroups = DecodeScoreOfKPI(result.KPIGroups);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                }
            }

            return result;
        }

        public CommandResult UpdateEmployeeEPA(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetMember(GetMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria.IsValid())
            {
                employees = this.GetData<Employee>(criteria);
            }

            return employees;
        }

        public IEnumerable<MemberEPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<MemberEPAModel> memberEPAs = new List<MemberEPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPAByEvaluator(criteria);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.SelfEvaluationScore = !string.IsNullOrEmpty(memberEPA.SelfEvaluationScore) ? Decode(memberEPA.SelfEvaluationScore) : string.Empty;
                    memberEPA.EvaluationScore = !string.IsNullOrEmpty(memberEPA.EvaluationScore) ? Decode(memberEPA.EvaluationScore) : string.Empty;
                }
            }

            return memberEPAs;
        }

        public CommandResult UpdateStatusEPA(UpdateStatusEPACriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public string Encode(string plainText)
        {
            return _encodeService.Encode(plainText);
        }

        public string Decode(string cipherText)
        {
            return _encodeService.Decode(cipherText);
        }

        public string ConvertKPIInfoToXml(IList<KPIGroupViewModel> kpiGroups)
        {
            string beginRootTag = "<ROOT>";
            string endRootTag = "</ROOT>";
            string employeeEPAScore = "<KPI KPIId='{0}' KPIGroupId='{1}' Score='{2}' Rate='{3}' Comment='{4}'></KPI>";
            var epaXML = new StringBuilder(beginRootTag);

            foreach (KPIGroupViewModel kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpiGroup.KPIs)
                {
                    string cipherScore = !string.IsNullOrEmpty(kpi.Score) ? Encode(kpi.Score + "_" + kpi.KPIId) : null;
                    epaXML.AppendFormat(employeeEPAScore, kpi.KPIId, kpiGroup.KPIGroupId, cipherScore, kpi.Rate, HttpUtility.HtmlEncode(kpi.Comment));
                }
            }

            epaXML = epaXML.Append(endRootTag);

            return epaXML.ToString();
        }

        private IList<KPIGroupViewModel> DecodeScoreOfKPI(IList<KPIGroupViewModel> kpiGroups)
        {
            foreach (KPIGroupViewModel kpiGroup in kpiGroups)
            {
                foreach (KPIModel kpi in kpiGroup.KPIs)
                {
                    string plainScore = !string.IsNullOrEmpty(kpi.Score) ? Decode(kpi.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                    kpi.Score = plainScore;
                }
            }

            return kpiGroups;
        }
    }
}
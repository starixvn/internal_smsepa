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
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
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
                    result.TotalScoreBySelf = !string.IsNullOrEmpty(result.TotalScoreBySelf) ? Decode(result.TotalScoreBySelf) : string.Empty;
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

        public EmployeeEPAInfoModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            EmployeeEPAInfoModel result = new EmployeeEPAInfoModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfoByEvaluator(criteria);

                if (result != null)
                {
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.KPIGroups = DecodeScoreOfKPI(result.KPIGroups);
                    result.TotalScoreBySelf = !string.IsNullOrEmpty(result.TotalScoreBySelf) ? Decode(result.TotalScoreBySelf) : string.Empty;
                    result.TotalScoreByEvaluator = !string.IsNullOrEmpty(result.TotalScoreByEvaluator) ? Decode(result.TotalScoreByEvaluator) : string.Empty;
                }
            }

            return result;
        }

        public IEnumerable<MemberEPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<MemberEPAModel> memberEPAs = new List<MemberEPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPAByReviewer(criteria);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.SelfEvaluationScore = !string.IsNullOrEmpty(memberEPA.SelfEvaluationScore) ? Decode(memberEPA.SelfEvaluationScore) : string.Empty;
                    memberEPA.ReviewerScore = !string.IsNullOrEmpty(memberEPA.ReviewerScore) ? Decode(memberEPA.ReviewerScore) : string.Empty;

                    foreach (var evaluator in memberEPA.Evaluators)
                    {
                        evaluator.EvaluationScore = !string.IsNullOrEmpty(evaluator.EvaluationScore) ? Decode(evaluator.EvaluationScore) : string.Empty;
                    }
                }

                AverageScore(memberEPAs);
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

        private IList<KPIGroupViewModel> DecodeScoreOfKPI(IList<KPIGroupViewModel> kpiGroups)
        {
            foreach (KPIGroupViewModel kpiGroup in kpiGroups)
            {
                foreach (KPIViewModel kpi in kpiGroup.KPIs)
                {
                    kpi.ScoreBySelf = !string.IsNullOrEmpty(kpi.ScoreBySelf) ? Decode(kpi.ScoreBySelf).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                    kpi.ScoreByEvaluator = !string.IsNullOrEmpty(kpi.ScoreByEvaluator) ? Decode(kpi.ScoreByEvaluator).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                }
            }

            return kpiGroups;
        }

        private IEnumerable<MemberEPAModel> AverageScore(IEnumerable<MemberEPAModel> memberEPAs)
        {
            foreach (var memberEPA in memberEPAs)
            {
                float score = 0;
                int count = 0;

                foreach (var evaluator in memberEPA.Evaluators)
                {
                    if (!string.IsNullOrEmpty(evaluator.EvaluationScore))
                    {
                        count++;
                        score += float.Parse(evaluator.EvaluationScore);
                    }
                }

                if (count > 0)
                {
                    memberEPA.EvaluationScore = (score / count).ToString();
                }
                else
                {
                    memberEPA.EvaluationScore = string.Empty;
                }
            }

            return memberEPAs;
        }
    }
}
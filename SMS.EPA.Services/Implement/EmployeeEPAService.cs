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

        #region Self

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            if (criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEPA(criteria);

                foreach (var epa in epas)
                {
                    epa.Status.Name = Enumerations.GetDescription((Status)epa.Status.StatusId);
                    epa.FinalScore = !string.IsNullOrEmpty(epa.FinalScore) ? Decode(epa.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(epa.ProcessBySelf);
                }
            }

            return epas;
        }

        public EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria)
        {
            EPAModel result = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfo(criteria);

                if (result != null)
                {
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(result.ProcessBySelf);

                    foreach (var info in result.KPIGroups)
                    {
                        foreach (var kpi in info.KPIs)
                        {
                            kpi.EvaluateBySelf.Score = !string.IsNullOrEmpty(kpi.EvaluateBySelf.Score) ? Decode(kpi.EvaluateBySelf.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                        }
                    }
                }
            }

            return result;
        }

        #endregion Self

        #region Evaluator

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessBySelf);
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByEvaluators);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            EPAModel result = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfo(criteria);

                if (result != null)
                {
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(result.ProcessBySelf);
                    DecodeTotalScoreAndGetStatus(result.ProcessByEvaluators);
                    DecodeScoreOnEachKPI(result.KPIGroups);
                }
            }

            return result;
        }

        #endregion Evaluator

        #region Reviewer

        public IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessBySelf);
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByEvaluators);
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByReviewers);
                    memberEPA.AverageScoreByEvaluators = AverageTotalScore(memberEPA.ProcessByEvaluators);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            EPAModel result = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfo(criteria);

                if (result != null)
                {
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(result.ProcessBySelf);
                    DecodeTotalScoreAndGetStatus(result.ProcessByEvaluators);
                    DecodeTotalScoreAndGetStatus(result.ProcessByReviewers);
                    result.AverageScoreByEvaluators = AverageTotalScore(result.ProcessByEvaluators);
                    DecodeScoreOnEachKPI(result.KPIGroups);
                }
            }

            return result;
        }

        #endregion Reviewer

        #region Management

        public IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessBySelf);
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByEvaluators);
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByReviewers);
                    DecodeTotalScoreAndGetStatus(memberEPA.ProcessByManagements);
                    memberEPA.AverageScoreByEvaluators = AverageTotalScore(memberEPA.ProcessByEvaluators);
                    memberEPA.AverageScoreByReviewers = AverageTotalScore(memberEPA.ProcessByReviewers);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria)
        {
            EPAModel result = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetEPAInfo(criteria);

                if (result != null)
                {
                    result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                    result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatus(result.ProcessBySelf);
                    DecodeTotalScoreAndGetStatus(result.ProcessByEvaluators);
                    DecodeTotalScoreAndGetStatus(result.ProcessByReviewers);
                    DecodeTotalScoreAndGetStatus(result.ProcessByManagements);
                    result.AverageScoreByEvaluators = AverageTotalScore(result.ProcessByEvaluators);
                    result.AverageScoreByReviewers = AverageTotalScore(result.ProcessByReviewers);
                    DecodeScoreOnEachKPI(result.KPIGroups);
                }
            }

            return result;
        }

        #endregion Management

        public CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateScoreByReviewerOrManagement(UpdateScoreByReviewerOrManagementCriteria criteria)
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

        private void DecodeTotalScoreAndGetStatus(EPAProcessModel epaProcess)
        {
            epaProcess.Status.Name = Enumerations.GetDescription((Status)epaProcess.Status.StatusId);
            epaProcess.TotalScore = !string.IsNullOrEmpty(epaProcess.TotalScore) ? Decode(epaProcess.TotalScore) : string.Empty;
        }

        private void DecodeTotalScoreAndGetStatus(IList<EPAProcessModel> epaProcesses)
        {
            foreach (var epaProcess in epaProcesses)
            {
                epaProcess.Status.Name = Enumerations.GetDescription((Status)epaProcess.Status.StatusId);
                epaProcess.TotalScore = !string.IsNullOrEmpty(epaProcess.TotalScore) ? Decode(epaProcess.TotalScore) : string.Empty;
            }
        }

        private void DecodeScoreOnEachKPI(IList<KPIGroupViewModel> kpiGroups)
        {
            foreach (var kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpiGroup.KPIs)
                {
                    kpi.EvaluateBySelf.Score = !string.IsNullOrEmpty(kpi.EvaluateBySelf.Score) ? Decode(kpi.EvaluateBySelf.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                    DecodeScore(kpi.EvaluateByEvaluators);
                }
            }
        }

        private void DecodeScore(IList<EvaluateModel> evaluates)
        {
            foreach (var evaluate in evaluates)
            {
                evaluate.Score = !string.IsNullOrEmpty(evaluate.Score) ? Decode(evaluate.Score).Replace("_" + evaluate.KPIId, string.Empty) : string.Empty;
            }
        }

        private string AverageTotalScore(IList<EPAProcessModel> epaProcesses)
        {
            float score = 0;
            int count = 0;

            foreach (var epaProcess in epaProcesses)
            {
                if (!string.IsNullOrEmpty(epaProcess.TotalScore))
                {
                    count++;
                    score += float.Parse(epaProcess.TotalScore);
                }
            }

            if (count > 0)
            {
                return (score / count).ToString();
            }

            return string.Empty;
        }
    }
}
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
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;
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
                RawEPAModel rawEPAModel = _employeeEPARepository.GetEPA(criteria);

                epas = rawEPAModel.Epas;
                GroupEmployeeByEPA(epas, rawEPAModel.Employees);
                GroupEPAProcessByEPA(epas, rawEPAModel.Processes);
                GroupEvaluatorByEPA(epas, rawEPAModel.Evaluators);

                foreach (var epa in epas)
                {
                    epa.Status.Name = Enumerations.GetDescription((Status)epa.Status.StatusId);
                    epa.FinalScore = !string.IsNullOrEmpty(epa.FinalScore) ? Decode(epa.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatusOnEachProcess(epa);
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
                RawEPAInfoModel rawEPAInfoModel = _employeeEPARepository.GetEPAInfo(criteria);

                result = rawEPAInfoModel.EPA;
                result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;

                GroupEvaluatorByProcess(result, rawEPAInfoModel.Evaluators);
                GroupEPAProcessByEPA(result, rawEPAInfoModel.Processes);
                GroupKPIByKPIGroup(result.KPIGroups, rawEPAInfoModel.KPIs);
                DecodeTotalScoreAndGetStatusOnEachProcess(result);

                foreach (var kpiGroup in result.KPIGroups)
                {
                    GroupEvaluationAndDecodeScoreOnEachKPI(kpiGroup.KPIs, rawEPAInfoModel.Evaluations);
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
                RawEPAModel rawEPAModel = _employeeEPARepository.GetEPA(criteria);

                memberEPAs = rawEPAModel.Epas;
                GroupEmployeeByEPA(memberEPAs, rawEPAModel.Employees);
                GroupEPAProcessByEPA(memberEPAs, rawEPAModel.Processes);
                GroupEvaluatorByEPA(memberEPAs, rawEPAModel.Evaluators);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatusOnEachProcess(memberEPA);
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
                RawEPAInfoModel rawEPAInfoModel = _employeeEPARepository.GetEPAInfo(criteria);

                result = rawEPAInfoModel.EPA;
                result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;

                GroupEvaluatorByProcess(result, rawEPAInfoModel.Evaluators);
                GroupEPAProcessByEPA(result, rawEPAInfoModel.Processes);
                GroupKPIByKPIGroup(result.KPIGroups, rawEPAInfoModel.KPIs);
                DecodeTotalScoreAndGetStatusOnEachProcess(result);

                foreach (var kpiGroup in result.KPIGroups)
                {
                    GroupEvaluationAndDecodeScoreOnEachKPI(kpiGroup.KPIs, rawEPAInfoModel.Evaluations);
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
                RawEPAModel rawEPAModel = _employeeEPARepository.GetEPA(criteria);

                memberEPAs = rawEPAModel.Epas;
                GroupEmployeeByEPA(memberEPAs, rawEPAModel.Employees);
                GroupEPAProcessByEPA(memberEPAs, rawEPAModel.Processes);
                GroupEvaluatorByEPA(memberEPAs, rawEPAModel.Evaluators);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatusOnEachProcess(memberEPA);
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
                RawEPAInfoModel rawEPAInfoModel = _employeeEPARepository.GetEPAInfo(criteria);

                result = rawEPAInfoModel.EPA;
                result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;

                GroupEvaluatorByProcess(result, rawEPAInfoModel.Evaluators);
                GroupEPAProcessByEPA(result, rawEPAInfoModel.Processes);
                GroupKPIByKPIGroup(result.KPIGroups, rawEPAInfoModel.KPIs);
                DecodeTotalScoreAndGetStatusOnEachProcess(result);
                result.AverageScoreByEvaluators = AverageTotalScore(result.ProcessByEvaluators);

                foreach (var kpiGroup in result.KPIGroups)
                {
                    GroupEvaluationAndDecodeScoreOnEachKPI(kpiGroup.KPIs, rawEPAInfoModel.Evaluations);
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
                RawEPAModel rawEPAModel = _employeeEPARepository.GetEPA(criteria);

                memberEPAs = rawEPAModel.Epas;
                GroupEmployeeByEPA(memberEPAs, rawEPAModel.Employees);
                GroupEPAProcessByEPA(memberEPAs, rawEPAModel.Processes);
                GroupEvaluatorByEPA(memberEPAs, rawEPAModel.Evaluators);

                foreach (var memberEPA in memberEPAs)
                {
                    memberEPA.Status.Name = Enumerations.GetDescription((Status)memberEPA.Status.StatusId);
                    memberEPA.FinalScore = !string.IsNullOrEmpty(memberEPA.FinalScore) ? Decode(memberEPA.FinalScore) : string.Empty;
                    DecodeTotalScoreAndGetStatusOnEachProcess(memberEPA);
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
                RawEPAInfoModel rawEPAInfoModel = _employeeEPARepository.GetEPAInfo(criteria);

                result = rawEPAInfoModel.EPA;
                result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;

                GroupEvaluatorByProcess(result, rawEPAInfoModel.Evaluators);
                GroupEPAProcessByEPA(result, rawEPAInfoModel.Processes);
                GroupKPIByKPIGroup(result.KPIGroups, rawEPAInfoModel.KPIs);
                DecodeTotalScoreAndGetStatusOnEachProcess(result);
                result.AverageScoreByEvaluators = AverageTotalScore(result.ProcessByEvaluators);
                result.AverageScoreByReviewers = AverageTotalScore(result.ProcessByReviewers);

                foreach (var kpiGroup in result.KPIGroups)
                {
                    GroupEvaluationAndDecodeScoreOnEachKPI(kpiGroup.KPIs, rawEPAInfoModel.Evaluations);
                }
            }

            return result;
        }

        #endregion Management

        #region Rating By KPI

        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria.IsValid())
            {
                targetTemplates = this.GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria.IsValid())
            {
                kpis = _employeeEPARepository.GetKPIByTargetTemplate(criteria);
            }

            return kpis;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEmployeeEPAByKPI(criteria);

                foreach (var epa in epas)
                {
                    epa.TotalScore = !string.IsNullOrEmpty(epa.TotalScore) ? Decode(epa.TotalScore) : string.Empty;

                    foreach (var kpi in epa.KPIGroup.KPIs)
                    {
                        kpi.Score = !string.IsNullOrEmpty(kpi.Score) ? Decode(kpi.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                    }
                }
            }

            return epas;
        }

        public CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        #endregion Rating By KPI

        public EPAModel GetEPAInfoById(GetEPAInfoByIdCriteria criteria)
        {
            EPAModel result = new EPAModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                RawEPAInfoModel rawEPAInfoModel = _employeeEPARepository.GetEPAInfo(criteria);

                result = rawEPAInfoModel.EPA;
                result.Status.Name = Enumerations.GetDescription((Status)result.Status.StatusId);
                result.FinalScore = !string.IsNullOrEmpty(result.FinalScore) ? Decode(result.FinalScore) : string.Empty;

                GroupEvaluatorByProcess(result, rawEPAInfoModel.Evaluators);
                GroupEPAProcessByEPA(result, rawEPAInfoModel.Processes);
                GroupKPIByKPIGroup(result.KPIGroups, rawEPAInfoModel.KPIs);
                DecodeTotalScoreAndGetStatusOnEachProcess(result);
                result.AverageScoreByEvaluators = AverageTotalScore(result.ProcessByEvaluators);
                result.AverageScoreByReviewers = AverageTotalScore(result.ProcessByReviewers);
                result.AverageScoreByManagements = AverageTotalScore(result.ProcessByManagements);

                foreach (var kpiGroup in result.KPIGroups)
                {
                    GroupEvaluationAndDecodeScoreOnEachKPI(kpiGroup.KPIs, rawEPAInfoModel.Evaluations);
                }
            }

            return result;
        }

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

        public string Encode(string plainText)
        {
            return _encodeService.Encode(plainText);
        }

        public string Decode(string cipherText)
        {
            return _encodeService.Decode(cipherText);
        }

        public void GroupEmployeeByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (var epa in epas)
            {
                foreach (var employee in employees)
                {
                    if (employee.EPAId == epa.EPAId)
                    {
                        epa.Employee = employee;
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epas, IEnumerable<EPAProcessModel> processes)
        {
            foreach (var epa in epas)
            {
                foreach (var process in processes)
                {
                    if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Self)
                    {
                        epa.ProcessBySelf = process;
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Evaluator)
                    {
                        epa.ProcessByEvaluators.Add(process);
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Reviewer)
                    {
                        epa.ProcessByReviewers.Add(process);
                    }
                    else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Management)
                    {
                        epa.ProcessByManagements.Add(process);
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(EPAModel epa, IEnumerable<EPAProcessModel> processes)
        {
            foreach (var process in processes)
            {
                if (process.EPAId == epa.EPAId
                    && process.ProcessId == (int)ProcessType.Self)
                {
                    epa.ProcessBySelf = process;
                }
                else if (process.EPAId == epa.EPAId
                    && process.ProcessId == (int)ProcessType.Evaluator)
                {
                    epa.ProcessByEvaluators.Add(process);
                }
                else if (process.EPAId == epa.EPAId
                    && process.ProcessId == (int)ProcessType.Reviewer)
                {
                    epa.ProcessByReviewers.Add(process);
                }
                else if (process.EPAId == epa.EPAId
                        && process.ProcessId == (int)ProcessType.Management)
                {
                    epa.ProcessByManagements.Add(process);
                }
            }
        }

        private void GroupEvaluatorByEPA(IEnumerable<EPAModel> epas, IEnumerable<EvaluatorModel> evaluators)
        {
            foreach (var epa in epas)
            {
                foreach (var evaluator in evaluators)
                {
                    if (evaluator.EPAId == epa.EPAId)
                    {
                        GroupEvaluatorByProcess(epa, evaluator);
                    }
                }
            }
        }

        private void GroupEvaluatorByProcess(EPAModel epa, EvaluatorModel evaluator)
        {
            if (evaluator.ProcessId == (int)ProcessType.Evaluator)
            {
                epa.Evaluators.Add(evaluator);
            }
            else if (evaluator.ProcessId == (int)ProcessType.Reviewer)
            {
                epa.Reviewers.Add(evaluator);
            }
            else if (evaluator.ProcessId == (int)ProcessType.Management)
            {
                epa.Managements.Add(evaluator);
            }
        }

        private void GroupEvaluatorByProcess(EPAModel epa, IEnumerable<EvaluatorModel> evaluators)
        {
            foreach (var evaluator in evaluators)
            {
                if (evaluator.EPAId == epa.EPAId)
                {
                    GroupEvaluatorByProcess(epa, evaluator);
                }
            }
        }

        private void GroupKPIByKPIGroup(IList<EPAKPIGroupModel> kpiGroups, IEnumerable<EPAKPIModel> kpis)
        {
            foreach (var kpiGroup in kpiGroups)
            {
                foreach (var kpi in kpis)
                {
                    if (kpi.EPAId == kpiGroup.EPAId
                        && kpi.KPIGroupId == kpiGroup.KPIGroupId)
                    {
                        kpiGroup.KPIs.Add(kpi);
                    }
                }
            }
        }

        private void GroupEvaluationAndDecodeScoreOnEachKPI(IEnumerable<EPAKPIModel> kpis, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (var kpi in kpis)
            {
                foreach (var evaluation in evaluations)
                {
                    EvaluationModel temp = CreateEvaluationModel(evaluation);
                    temp.Score = !string.IsNullOrEmpty(temp.Score) ? Decode(temp.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;

                    if (temp.ProcessId == (int)ProcessType.Self
                        && temp.EPAId == kpi.EPAId
                        && temp.KPIId == kpi.KPIId)
                    {
                        kpi.EvaluateBySelf = temp;
                    }
                    else if (temp.ProcessId == (int)ProcessType.Evaluator
                        && temp.EPAId == kpi.EPAId
                        && temp.KPIId == kpi.KPIId)
                    {
                        kpi.EvaluateByEvaluators.Add(temp);
                    }
                }
            }
        }

        private EvaluationModel CreateEvaluationModel(EvaluationModel evaluation)
        {
            EvaluationModel result = new EvaluationModel()
            {
                EPAId = evaluation.EPAId,
                ProcessId = evaluation.ProcessId,
                KPIId = evaluation.KPIId,
                EvaluatorId = evaluation.EvaluatorId,
                Code = evaluation.Code,
                NickName = evaluation.NickName,
                FullName = evaluation.FullName,
                Score = evaluation.Score,
                Rate = evaluation.Rate,
                Comment = evaluation.Comment
            };

            return result;
        }

        private void DecodeTotalScoreAndGetStatusOnEachProcess(EPAModel epa)
        {
            epa.ProcessBySelf.Status.Name = Enumerations.GetDescription((Status)epa.ProcessBySelf.Status.StatusId);
            epa.ProcessBySelf.TotalScore = !string.IsNullOrEmpty(epa.ProcessBySelf.TotalScore) ? Decode(epa.ProcessBySelf.TotalScore) : string.Empty;

            foreach (var epaProcess in epa.ProcessByEvaluators)
            {
                epaProcess.Status.Name = Enumerations.GetDescription((Status)epaProcess.Status.StatusId);
                epaProcess.TotalScore = !string.IsNullOrEmpty(epaProcess.TotalScore) ? Decode(epaProcess.TotalScore) : string.Empty;
            }

            foreach (var epaProcess in epa.ProcessByReviewers)
            {
                epaProcess.Status.Name = Enumerations.GetDescription((Status)epaProcess.Status.StatusId);
                epaProcess.TotalScore = !string.IsNullOrEmpty(epaProcess.TotalScore) ? Decode(epaProcess.TotalScore) : string.Empty;
            }

            foreach (var epaProcess in epa.ProcessByManagements)
            {
                epaProcess.Status.Name = Enumerations.GetDescription((Status)epaProcess.Status.StatusId);
                epaProcess.TotalScore = !string.IsNullOrEmpty(epaProcess.TotalScore) ? Decode(epaProcess.TotalScore) : string.Empty;
            }
        }

        private void DecodeTotalScoreAndScoreOnEachKPI(IEnumerable<RatingByKPIModel> eaps)
        {
            foreach (var epa in eaps)
            {
                epa.TotalScore = !string.IsNullOrEmpty(epa.TotalScore) ? Decode(epa.TotalScore) : string.Empty;

                foreach (var kpi in epa.KPIGroup.KPIs)
                {
                    kpi.Score = !string.IsNullOrEmpty(kpi.Score) ? Decode(kpi.Score).Replace("_" + kpi.KPIId, string.Empty) : string.Empty;
                }
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
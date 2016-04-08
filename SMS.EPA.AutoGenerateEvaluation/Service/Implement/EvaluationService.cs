// ----------------------------------------------------------------------
// <copyright file="EvaluationService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/26/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Criteria.Monthly;
    using SMS.EPA.AutoGenerateEvaluation.Models;
    using SMS.EPA.AutoGenerateEvaluation.Repository;
    using SMS.EPA.AutoGenerateEvaluation.Service.Interfaces;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Period;

    public class EvaluationService : BaseService, IEvaluationService
    {
        private readonly IEvaluationRepository _generateEvaluationRepository;

        public EvaluationService()
            : this(null)
        {
        }

        public EvaluationService(IEvaluationRepository generationEvaluationRepository)
        {
            _generateEvaluationRepository = generationEvaluationRepository ?? new EvaluationRepository();
        }

        #region Semi Annual

        public IEnumerable<int> CheckStartSemiAnnual(GetSemiAnnualPeriodStartedCriteria criteria)
        {
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        #endregion Semi Annual

        #region Probation

        public AutoEmployeePeriodModel GetEmployeeProbationEvaluation(GetEmployeeProbationEvaluationCriteria criteria)
        {
            AutoEmployeePeriodModel employeePeriod = new AutoEmployeePeriodModel();

            if (criteria != null && criteria.IsValid())
            {
                employeePeriod = _generateEvaluationRepository.GetEmployeeProbationEvaluation(criteria);
            }

            return employeePeriod;
        }

        public CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Insert(criteria);
            }

            return comandResult;
        }

        #endregion Probation

        #region Promotion

        public IEnumerable<AutoEmployeePeriodModel> GetEmployeePromotionEvaluation(GetEmployeePromotionEvaluationCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeePeriods = GetData<AutoEmployeePeriodModel>(criteria);
                employeePeriods = employeePeriods.GroupBy(x => x.PeriodId).Select(x => new AutoEmployeePeriodModel
                {
                    PeriodId = x.Key,
                    EmployeeIds = JoinEmployeeId(x.ToList())
                });
            }

            return employeePeriods;
        }

        #endregion Promotion

        #region Monthly

        public IEnumerable<int> GetMonthlyPeriodStarted(GetMonthlyPeriodStartedCriteria criteria)
        {
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToMonthlyPeriod(InsertEmployeeToMonthlyPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        #endregion Monthly

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> reminderPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                reminderPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return reminderPeriods;
        }

        public IEnumerable<AutoEmployeePeriodModel> CheckEndProcess(CheckEndProcessCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                evaluationProcesses = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return evaluationProcesses;
        }

        public CommandResult UpdateEndProcess(UpdateIsEndProcessCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public CommandResult UpdateStatusEndEvaluaitonPeriod(UpdateStatusEndEvaluationPeriodCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public int CheckLastestProcessOfPeriod(CheckLastestProcessOfPeriodCriteria criteria)
        {
            int lastestProcess = 0;

            if (criteria != null && criteria.IsValid())
            {
                lastestProcess = GetDataEntity<int>(criteria);
            }

            return lastestProcess;
        }

        public bool CheckTotalScoreOfProcessIsNull(CheckTotalScoreOfProcessIsNullCriteria criteria)
        {
            bool isTotalScoreNull = false;

            if (criteria != null && criteria.IsValid())
            {
                isTotalScoreNull = GetDataEntity<bool>(criteria);
            }

            return isTotalScoreNull;
        }

        public CommandResult UpdateTotalScoreOfProcess(UpdateTotalScoreOfProcessCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<EndEPAModel> GetEvaluationEndedYesterday(GetEvaluationEndedYesterdayCriteria criteria)
        {
            IEnumerable<EndEPAModel> evaluations = new List<EndEPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                evaluations = GetData<EndEPAModel>(criteria);
            }

            return evaluations;
        }

        public CommandResult UpdateTotalScoreOfSemiHasMonthly(UpdateTotalScoreOfSemiHasMonthlyCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private string JoinEmployeeId(List<AutoEmployeePeriodModel> employeePeriods)
        {
            string employeeIds = string.Join(",", employeePeriods.Select(x => x.EmployeeId.ToString()));

            return employeeIds;
        }
    }
}
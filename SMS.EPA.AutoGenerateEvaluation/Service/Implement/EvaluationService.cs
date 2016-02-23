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
    using Fanex.Common.Helper;
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
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        #endregion Semi Annual

        #region Probation

        public AutoEmployeePeriodModel GetEmployeeProbationEvaluation(GetEmployeeProbationEvaluationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var employeePeriod = new AutoEmployeePeriodModel();

            if (criteria.IsValid())
            {
                employeePeriod = _generateEvaluationRepository.GetEmployeeProbationEvaluation(criteria);
            }

            return employeePeriod;
        }

        public CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Insert(criteria);
            }

            return comandResult;
        }

        #endregion Probation

        #region Promotion

        public IEnumerable<AutoEmployeePeriodModel> GetEmployeePromotionEvaluation(GetEmployeePromotionEvaluationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
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
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToMonthlyPeriod(InsertEmployeeToMonthlyPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        #endregion Monthly

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AutoEmployeePeriodModel> reminderPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                reminderPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return reminderPeriods;
        }

        public bool CheckPeriodStartedSendMailAll(CheckPeriodStartedSendMailAllCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool hasSendMailAll = false;

            if (criteria.IsValid())
            {
                hasSendMailAll = GetDataEntity<bool>(criteria);
            }

            return hasSendMailAll;
        }

        public IEnumerable<AutoEmployeePeriodModel> CheckEndProcess(CheckEndProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                evaluationProcesses = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return evaluationProcesses;
        }

        public CommandResult UpdateEndProcess(UpdateIsEndProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public IEnumerable<EndEPAModel> CheckEndEvaluationPeriod(CheckEndEvaluationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EndEPAModel> evaluations = new List<EndEPAModel>();

            if (criteria.IsValid())
            {
                evaluations = GetData<EndEPAModel>(criteria);
            }

            return evaluations;
        }

        public CommandResult UpdateStatusEndEvaluaitonPeriod(UpdateStatusEndEvaluationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public int CheckLastestProcessOfPeriod(CheckLastestProcessOfPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int lastestProcess = 0;

            if (criteria.IsValid())
            {
                lastestProcess = GetDataEntity<int>(criteria);
            }

            return lastestProcess;
        }

        public bool CheckTotalScoreOfProcessIsNull(CheckTotalScoreOfProcessIsNullCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool isTotalScoreNull = false;

            if (criteria.IsValid())
            {
                isTotalScoreNull = GetDataEntity<bool>(criteria);
            }

            return isTotalScoreNull;
        }

        public CommandResult UpdateTotalScoreOfProcess(UpdateTotalScoreOfProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<EndEPAModel> GetEvaluationEndedYesterday(GetEvaluationEndedYesterdayCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<EndEPAModel> evaluations = new List<EndEPAModel>();

            if (criteria.IsValid())
            {
                evaluations = GetData<EndEPAModel>(criteria);
            }

            return evaluations;
        }

        private string JoinEmployeeId(List<AutoEmployeePeriodModel> employeePeriods)
        {
            string employeeIds = string.Empty;

            foreach (AutoEmployeePeriodModel employeePeriod in employeePeriods)
            {
                employeeIds += !string.IsNullOrWhiteSpace(employeeIds) ? "," + employeePeriod.EmployeeId.ToString() : employeePeriod.EmployeeId.ToString();
            }

            return employeeIds;
        }
    }
}
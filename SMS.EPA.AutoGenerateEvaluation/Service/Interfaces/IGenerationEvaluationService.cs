// ----------------------------------------------------------------------
// <copyright file="IGenerationEvaluationService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/01/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Service.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Model;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Period;

    public interface IGenerationEvaluationService
    {
        IEnumerable<int> CheckStartSemiAnnual(GetSemiAnnualPeriodStartedCriteria criteria);

        CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria);

        CommandResult AutoGenerateEvaluation(AutoGenerateEvaluationCriteria criteria);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria);

        AutoEmployeePeriodModel GetEmployeeProbationEvaluation(GetEmployeeProbationEvaluationCriteria criteria);

        CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(GetPeriodHasStartedProcessCriteria criteria);

        bool CheckPeriodStartedSendMailAll(CheckPeriodStartedSendMailAllCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria);
    }
}

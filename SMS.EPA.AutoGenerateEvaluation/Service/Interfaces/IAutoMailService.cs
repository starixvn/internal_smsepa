// ----------------------------------------------------------------------
// <copyright file="IAutoMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/03/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Service.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Period;

    public interface IAutoMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore);
    }
}

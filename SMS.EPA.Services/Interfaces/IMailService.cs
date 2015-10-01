// ----------------------------------------------------------------------
// <copyright file="IMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/28/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Period;

    public interface IMailService
    {
        void SendMailToHRStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfEndEvaluation(string templateEmailPath, EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore);
    }
}

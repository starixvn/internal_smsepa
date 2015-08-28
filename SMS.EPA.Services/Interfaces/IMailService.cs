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
    using SMS.EPA.Model.Period;

    public interface IMailService
    {
        void SendMailToHRStartDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfStartDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorStartDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerStartDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementStartDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfRemindDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorRemindDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerRemindDate(PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementRemindDate(PeriodFlowModel employeeSetting, string periodName);
    }
}

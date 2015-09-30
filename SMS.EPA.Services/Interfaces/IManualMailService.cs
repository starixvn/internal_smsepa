// ----------------------------------------------------------------------
// <copyright file="IManualMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/04/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Period;

    public interface IManualMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}

// ----------------------------------------------------------------------
// <copyright file="RawPeriodFlowSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    public class RawPeriodFlowSettingModel : PeriodFlowModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }
    }
}

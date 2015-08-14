// ----------------------------------------------------------------------
// <copyright file="RawPeriodEmployeeSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using System.Collections.Generic;

    public class RawPeriodEmployeeSettingModel
    {
        public IEnumerable<PeriodEmployeeSettingModel> Employees { get; set; }

        public IEnumerable<RawPeriodFlowSettingModel> FlowSettings { get; set; }
    }
}

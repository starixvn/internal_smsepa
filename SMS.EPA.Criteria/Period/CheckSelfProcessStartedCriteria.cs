// ----------------------------------------------------------------------
// <copyright file="CheckSelfProcessStartedCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/25/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class CheckSelfProcessStartedCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds);
        }

        public override string GetSettingKey()
        {
            return "EPAPeriod_CheckSelfProcessStarted";
        }
    }
}

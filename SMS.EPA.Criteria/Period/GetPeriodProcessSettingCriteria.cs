// ----------------------------------------------------------------------
// <copyright file="GetPeriodProcessSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/22/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetPeriodProcessSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetProcessSetting";
        }
    }
}

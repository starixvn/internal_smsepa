// ----------------------------------------------------------------------
// <copyright file="DeletePeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class DeletePeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_Delete";
        }
    }
}

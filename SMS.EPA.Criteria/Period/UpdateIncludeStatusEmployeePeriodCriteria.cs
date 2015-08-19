// ----------------------------------------------------------------------
// <copyright file="UpdateIncludeStatusEmployeePeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class UpdateIncludeStatusEmployeePeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public bool IsIncluded { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_UpdateIncludeStatusEmployeeToPeriod";
        }
    }
}

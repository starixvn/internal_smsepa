// ----------------------------------------------------------------------
// <copyright file="UpdateEmployeeSettingOfPeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class UpdateEmployeeSettingOfPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeIds { get; set; }

        public int? TargetTemplateId { get; set; }

        public string EmployeeAssignments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_EditEmployeeSetting";
        }
    }
}

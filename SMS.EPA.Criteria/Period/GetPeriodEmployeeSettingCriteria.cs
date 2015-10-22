// ----------------------------------------------------------------------
// <copyright file="GetPeriodEmployeeSettingCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetPeriodEmployeeSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string PeriodName { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsExcludedEmployee { get; set; }

        public bool? IsGeneratedEPA { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetEmployeeSetting";
        }
    }
}

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
    using System;
    using Fanex.Data.Repository;

    public class GetPeriodEmployeeSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string PeriodName { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public bool? IsExcludedEmployee { get; set; }

        public bool? IsGeneratedEPA { get; set; }

        public int? IsEvaluated { get; set; }

        [SpParam(Ignored = true)]
        public DateTime? MonthlyPeriod { get; set; }

        public int? Month
        {
            get
            {
                int? month = null;

                if (MonthlyPeriod != null)
                {
                    month = MonthlyPeriod.Value.Month;
                }

                return month;
            }
        }

        public int? Year
        {
            get
            {
                int? year = null;

                if (MonthlyPeriod != null)
                {
                    year = MonthlyPeriod.Value.Year;
                }

                return year;
            }
        }

        public bool? HasMonthlyEPA { get; set; }

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
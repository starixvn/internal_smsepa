// -----------------------------------------------------------------------
// <copyright file="GetMonthlyPeriodByIdCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.02.16">Created</change>
// </history>

namespace SMS.EPA.Criteria.MonthlyPeriod
{
    using Fanex.Data.Repository;

    public class GetMonthlyPeriodByIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaMonthlyPeriod_GetById";
        }
    }
}
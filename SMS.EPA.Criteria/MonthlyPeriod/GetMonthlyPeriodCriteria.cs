// -----------------------------------------------------------------------
// <copyright file="GetMonthlyPeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.02.15">Created</change>
// </history>

namespace SMS.EPA.Criteria.MonthlyPeriod
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyPeriodCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaMonthlyPeriod_Get";
        }
    }
}
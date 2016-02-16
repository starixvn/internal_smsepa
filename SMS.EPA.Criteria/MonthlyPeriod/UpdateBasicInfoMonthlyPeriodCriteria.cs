// -----------------------------------------------------------------------
// <copyright file="UpdateBasicInfoMonthlyPeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2016.02.16">Created</change>
// </history>

namespace SMS.EPA.Criteria.MonthlyPeriod
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateBasicInfoMonthlyPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "EpaMonthlyPeriod_UpdateBasicInfo";
        }
    }
}
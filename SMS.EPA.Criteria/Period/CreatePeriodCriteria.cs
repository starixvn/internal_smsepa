// ----------------------------------------------------------------------
// <copyright file="CreatePeriodCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Period
{
    using System;
    using Fanex.Data.Repository;

    public class CreatePeriodCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? AttendanceStartDate { get; set; }

        public DateTime? AttendanceEndDate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_Insert";
        }
    }
}

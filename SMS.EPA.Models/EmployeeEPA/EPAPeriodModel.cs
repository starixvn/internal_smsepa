// -----------------------------------------------------------------------
// <copyright file="EPAPeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.01">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;

    public class EPAPeriodModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsEndPeriod { get; set; }

        public int TargetTypeId { get; set; }

        public string TargetTypeName { get; set; }
    }
}
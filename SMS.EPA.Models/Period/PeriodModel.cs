// ----------------------------------------------------------------------
// <copyright file="PeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using System;
    using SMS.EPA.Model.TargetType;

    public class PeriodModel
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? AttendanceStartDate { get; set; }

        public DateTime? AttendanceEndDate { get; set; }

        public int NumberEmployees { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public int? Year
        {
            get
            {
                int? year = null;

                if (StartDate != null)
                {
                    year = StartDate.Value.Year;
                }

                return year;
            }
        }
    }
}
// ----------------------------------------------------------------------
// <copyright file="AutoEmployeePeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/04/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using SMS.EPA.Core.Services;

    public class AutoEmployeePeriodModel
    {
        public int EPAId { get; set; }

        public string EmployeeIds { get; set; }

        public int PeriodId { get; set; }

        public int TargetTypeId { get; set; }

        public int ProcessId { get; set; }

        public string TotalScoreEncode { get; set; }

        public string TotalScore
        {
            get
            {
                return EncodeService.Decode(TotalScoreEncode);
            }
        }

        public int EmployeeId { get; set; }

        public bool HasMonthlyEPA { get; set; }
    }
}
// ----------------------------------------------------------------------
// <copyright file="EPAHistoryModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/19/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.EPAHistory
{
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Period;

    public class EPAHistoryModel
    {
        public EmployeeModel Employee { get; set; }

        public PeriodModel Period { get; set; }

        public float FinalScore { get; set; }

        public string EncodeFinalScore { get; set; }

        public string Grade { get; set; }

        public string EncodeGrade { get; set; }
    }
}

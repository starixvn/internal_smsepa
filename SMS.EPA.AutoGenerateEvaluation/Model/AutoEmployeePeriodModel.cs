// ----------------------------------------------------------------------
// <copyright file="AutoEmployeePeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/14/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Model
{
    using System.Collections.Generic;

    public class AutoEmployeePeriodModel
    {
        public string EmployeeIds { get; set; }

        public int PeriodId { get; set; }

        public int ProcessId { get; set; }
    }
}

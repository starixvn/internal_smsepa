// -----------------------------------------------------------------------
// <copyright file="EPAProcessModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int Priority { get; set; }

        public string TotalScore { get; set; }

        public string Comment { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsEndProcess { get; set; }

        public StatusModel Status { get; set; }

        public DateTime? EvaluateDate { get; set; }

        public Employee Evaluator { get; set; }
    }
}
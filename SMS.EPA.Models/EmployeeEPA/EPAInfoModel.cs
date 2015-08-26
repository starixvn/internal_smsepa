// -----------------------------------------------------------------------
// <copyright file="EPAInfoModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class EPAInfoModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public Employee Evaluator { get; set; }

        public KPIGroupViewModel KPIGroup { get; set; }
    }
}
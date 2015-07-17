// -----------------------------------------------------------------------
// <copyright file="EPAFlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    using System;
    using SMS.EPA.Model.Employee;

    public class EPAFlowModel
    {
        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool HasSendMail { get; set; }

        public EmployeeModel EvaluatorInfo { get; set; }
    }
}
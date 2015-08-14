// -----------------------------------------------------------------------
// <copyright file="FlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.17">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    using SMS.EPA.Model.Employee;

    public class FlowModel
    {
        public int ProcessId { get; set; }

        public int Duration { get; set; }

        public int Priority { get; set; }

        public int RemindDay { get; set; }

        public bool HasSendMail { get; set; }

        public FlowTemplateModel FlowTemplate { get; set; }

        public EmployeeModel EvaluatorInfo { get; set; }
    }
}
// -----------------------------------------------------------------------
// <copyright file="ProcessFlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.26">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    public class ProcessFlowModel
    {
        public int DepartmentId { get; set; }

        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }
    }
}
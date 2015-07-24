// ----------------------------------------------------------------------
// <copyright file="RawEmployeeFlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.CheckFlowAssignment
{
    using SMS.EPA.Model.FlowManagement;

    public class RawEmployeeFlowModel : FlowModel
    {
        public int EmployeeId { get; set; }

        public int TargetTemplateId { get; set; }
    }
}

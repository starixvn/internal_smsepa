// ----------------------------------------------------------------------
// <copyright file="DepartmentFlowSetingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/29/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.CheckFlowAssignment
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.FlowManagement;

    public class DepartmentFlowSetingModel : Employee
    {
        public int DepartmentId { get; set; }

        public IList<FlowModel> Evaluators { get; set; }

        public IList<FlowModel> Reviewers { get; set; }

        public IList<FlowModel> Managements { get; set; }

        public IList<FlowModel> HumanResources { get; set; }
    }
}
